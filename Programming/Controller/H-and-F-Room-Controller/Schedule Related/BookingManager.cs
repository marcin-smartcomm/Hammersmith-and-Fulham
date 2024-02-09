using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace H_and_F_Room_Controller
{
    public class BookingManager
    {
        ControlSystem _cs;
        SSE_Server _eventServer;

        public BookingManager(SSE_Server eventServer, ControlSystem cs)
        {
            _cs = cs;
            _eventServer = eventServer;
        }

        public void ProcessBookings(int roomID, Bookings currentBookings)
        {
            //Add 1 Hour if daylight saving time
            currentBookings = AddHoursToStartAndEndTimes(1, currentBookings);
            FileOperations.saveRoomBookings(roomID, "Bookings", currentBookings);

            CurrentAndNextMeetingInfo newCurrentAndNextMeetingInfo = new CurrentAndNextMeetingInfo();
            MeetingDurations meetingDurations = new MeetingDurations();

            newCurrentAndNextMeetingInfo.dateNow = FillDateNowSection();

            int[] startTimes = GetAllStartTimes(currentBookings);
            int[] endTimes = GetAllEndTimes(currentBookings);

            FillOutMeetingInfoCards(currentBookings, roomID);

            meetingDurations.startTimesInMinutes = startTimes;
            meetingDurations.endTimesInMinutes = endTimes;
            meetingDurations = FillMeetingDurations(currentBookings, meetingDurations);

            newCurrentAndNextMeetingInfo.freeAllDay = CheckIfFreeAllDay(startTimes, endTimes);

            if (newCurrentAndNextMeetingInfo.freeAllDay)
            {
                ConsoleLogger.WriteLine("No More Meetings Today, refreshing calendar every 5 minute");
                _cs.refreshCalendarAfterMinutes = 5;

                FileOperations.saveRoomBookingStats(roomID, newCurrentAndNextMeetingInfo);
                FileOperations.saveMeetingDurations(roomID, meetingDurations);
                _eventServer.UpdateAllConnected(roomID, "Bookings");

                Task.Run(() => {
                    Thread.Sleep(500);
                    SendOffReq(roomID);
                });

                return;
            }
            else
            {
                newCurrentAndNextMeetingInfo.inMeeting = CheckIfInMeeting(startTimes, endTimes);

                if (newCurrentAndNextMeetingInfo.inMeeting)
                {
                    ConsoleLogger.WriteLine("In meeting, refreshing calendar every 1 minute");
                    _cs.refreshCalendarAfterMinutes = 1;

                    FillTimeRemainingInfo(newCurrentAndNextMeetingInfo, currentBookings);
                    FillCurrentMeetingInfo(newCurrentAndNextMeetingInfo, currentBookings);

                    //If current meeintg is not last
                    if (startTimes[startTimes.Length - 1] != startTimes[curretMeetingIndex])
                    {
                        FillTimeUntilInfo(newCurrentAndNextMeetingInfo, currentBookings, curretMeetingIndex + 1);
                        FillNextMeetingInfo(newCurrentAndNextMeetingInfo, currentBookings, curretMeetingIndex + 1);
                    }

                    Task.Run(() => {
                        Thread.Sleep(500);
                        SendPrepReq(roomID);
                    });
                }
                else //not in meeting so next meeting is first meeting on list
                {
                    int minutesUntilNextMeeting = FillTimeUntilInfo(newCurrentAndNextMeetingInfo, currentBookings, GetNextMeetingIndex(startTimes));
                    FillNextMeetingInfo(newCurrentAndNextMeetingInfo, currentBookings, GetNextMeetingIndex(startTimes));

                    if(minutesUntilNextMeeting <= 10)
                    {
                        Task.Run(() => {
                            Thread.Sleep(500);
                            SendPrepReq(roomID);
                        });

                        ConsoleLogger.WriteLine($"Next meeting in {minutesUntilNextMeeting}min, refereshing calendar every 1 minute");
                        _cs.refreshCalendarAfterMinutes = 1;
                    }
                    else
                    {
                        Task.Run(() => { 
                            Thread.Sleep(500);
                            SendOffReq(roomID);
                        });

                        ConsoleLogger.WriteLine("Not in meeting, refreshing calendar every 5 minute");
                        _cs.refreshCalendarAfterMinutes = 5;
                    }
                }
            }

            FileOperations.saveRoomBookingStats(roomID, newCurrentAndNextMeetingInfo);
            FileOperations.saveMeetingDurations(roomID, meetingDurations);

            Task.Run(() => { Thread.Sleep(500); _eventServer.UpdateAllConnected(roomID, "Bookings"); });

            PrintOutCurrentAndNextMeetingInfo(newCurrentAndNextMeetingInfo);
        }

        void SendPrepReq(int roomID)
        {
            Room foundRoom = _cs.roomManager.rooms.Find(x => x.GetRoomID() == roomID);
            if (!foundRoom.roomPreppedforMeeting) foundRoom.PrepRoomForMeeting();
            foundRoom.roomPreppedforMeeting = true;
        }

        void SendOffReq(int roomID)
        {
            Room foundRoom = _cs.roomManager.rooms.Find(x => x.GetRoomID() == roomID);
            if (foundRoom.roomPreppedforMeeting) foundRoom.Shutdown();
            foundRoom.roomPreppedforMeeting = false;
        }

        void PrintOutCurrentAndNextMeetingInfo(CurrentAndNextMeetingInfo newCurrentAndNextMeetingInfo)
        {

            ConsoleLogger.WriteLine("----- Current Booking Stats -----");
            ConsoleLogger.WriteLine(newCurrentAndNextMeetingInfo.currentMeetingStartEndTime);
            ConsoleLogger.WriteLine(newCurrentAndNextMeetingInfo.currentMeetingOrganiser);
            ConsoleLogger.WriteLine(newCurrentAndNextMeetingInfo.currentMeetingSubject);

            ConsoleLogger.WriteLine("----- Next Booking Stats -----");
            ConsoleLogger.WriteLine(newCurrentAndNextMeetingInfo.nextMeetingStartEndTime);
            ConsoleLogger.WriteLine(newCurrentAndNextMeetingInfo.nextMeetingOrganiser);
            ConsoleLogger.WriteLine(newCurrentAndNextMeetingInfo.nextMeetingSubject);
        }

        public void DecreaseTimeUntilNextMeetingMinutes(int roomID)
        {
            Bookings myBookings = JsonConvert.DeserializeObject<Bookings>(FileOperations.loadRoomJson(roomID, "Bookings"));
            CurrentAndNextMeetingInfo bookingsInfo = JsonConvert.DeserializeObject<CurrentAndNextMeetingInfo>(FileOperations.loadRoomJson(roomID, "BookingStats"));
            int[] startTimes = GetAllStartTimes(myBookings);

            FillTimeUntilInfo(bookingsInfo, myBookings, GetNextMeetingIndex(startTimes));

            FileOperations.saveRoomBookingStats(roomID, bookingsInfo);

            Task.Run(() => { Thread.Sleep(500); _eventServer.UpdateAllConnected(roomID, "Bookings"); }); 
        }

        Bookings AddHoursToStartAndEndTimes(int hours, Bookings currentBookings)
        {
            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo myTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
            ConsoleLogger.WriteLine($"UTC: {utc} --- Daylight Saving Time: {myTimeZone.IsDaylightSavingTime(utc)} ");

            if (myTimeZone.IsDaylightSavingTime(utc))
            {
                ConsoleLogger.WriteLine($"In Daylight Saving Time, adding 1 hour");
                foreach (Value booking in currentBookings.value)
                {
                    booking.start.dateTime = booking.start.dateTime.AddHours(hours);
                    booking.end.dateTime = booking.end.dateTime.AddHours(hours);
                }
            }

            return currentBookings;
        }

        MeetingDurations FillMeetingDurations(Bookings bookings, MeetingDurations meetingDurations)
        {
            meetingDurations.startHours = new int[bookings.value.Count];
            meetingDurations.startMinutes = new int[bookings.value.Count];
            meetingDurations.endHours = new int[bookings.value.Count];
            meetingDurations.endMinutes = new int[bookings.value.Count];

            for(int i = 0; i < bookings.value.Count; i++)
            {
                meetingDurations.startHours[i] = bookings.value[i].start.dateTime.Hour;
                meetingDurations.startMinutes[i] = bookings.value[i].start.dateTime.Minute;
                meetingDurations.endHours[i] = bookings.value[i].end.dateTime.Hour;
                meetingDurations.endMinutes[i] = bookings.value[i].end.dateTime.Minute;
            }

            return meetingDurations;
        }

        void FillOutMeetingInfoCards(Bookings currentBookings, int roomID)
        {
            MeetingInfoCardCollection newMeetingInfoCardCollection = new MeetingInfoCardCollection();
            newMeetingInfoCardCollection.meetingInfoCards = new List<MeetingInfoCard>();

            for(int i = 0; i < currentBookings.value.Count; i++)
            {
                MeetingInfoCard newMeetingInfoCard = new MeetingInfoCard();
                newMeetingInfoCard.meetingSubject = currentBookings.value[i].subject;
                newMeetingInfoCard.meetingOrganiser = currentBookings.value[i].organizer.emailAddress.name;
                newMeetingInfoCard.startHour = currentBookings.value[i].start.dateTime.Hour;
                newMeetingInfoCard.startMinute = currentBookings.value[i].start.dateTime.Minute;
                newMeetingInfoCard.endHour = currentBookings.value[i].end.dateTime.Hour;
                newMeetingInfoCard.endMinute = currentBookings.value[i].end.dateTime.Minute;
                newMeetingInfoCard.meetingDurationText = GetSpecificMeetingStartEndTime(currentBookings, i);

                newMeetingInfoCardCollection.meetingInfoCards.Add(newMeetingInfoCard);
            }

            FileOperations.saveMeetingInfoCards(roomID, newMeetingInfoCardCollection);
        }

        int[] GetAllStartTimes(Bookings bookings)
        {
            int[] startTimes = new int[bookings.value.Count];

            for(int i = 0; i < bookings.value.Count; i++)
                startTimes[i] = bookings.value[i].start.dateTime.Hour * 60 + bookings.value[i].start.dateTime.Minute;
            
            return startTimes;
        }

        int[] GetAllEndTimes(Bookings bookings)
        {
            int[] endTimes = new int[bookings.value.Count];

            for (int i = 0; i < bookings.value.Count; i++)
                endTimes[i] = bookings.value[i].end.dateTime.Hour * 60 + bookings.value[i].end.dateTime.Minute;

            return endTimes;
        }

        bool CheckIfFreeAllDay(int[] startTimes, int[] endTimes)
        {
            bool noStartTimesInFutre = true;
            bool noEndTimesInFutre = true;

            int timeNow = DateTime.Now.Hour*60+DateTime.Now.Minute;

            foreach (int startTime in startTimes)
                if (startTime >= timeNow)
                {
                    noStartTimesInFutre = false;
                    break;
                }

            foreach (int endTime in endTimes)
                if (endTime > timeNow)
                {
                    noEndTimesInFutre = false;
                    break;
                }

            if (noEndTimesInFutre && noStartTimesInFutre) return true;
            else return false;
        }

        int curretMeetingIndex = 0;
        bool CheckIfInMeeting(int[] startTimes, int[] endTimes)
        {
            bool inMeeting = false;

            int timeNow = DateTime.Now.Hour * 60 + DateTime.Now.Minute;

            for(int i = 0; i < startTimes.Length; i++)
                if (startTimes[i] <= timeNow)
                    if (endTimes[i] > timeNow)
                    {
                        inMeeting = true;
                        curretMeetingIndex = i;
                        break;
                    }

            return inMeeting;
        }

        void FillCurrentMeetingInfo(CurrentAndNextMeetingInfo newBookingStats, Bookings currentBookings)
        {
            newBookingStats.currentMeetingStartEndTime = GetCurrenMeetinStartEndTime(currentBookings);
            newBookingStats.currentMeetingID = currentBookings.value[curretMeetingIndex].id;
            newBookingStats.currentMeetingOrganiser = currentBookings.value[curretMeetingIndex].organizer.emailAddress.name;
            newBookingStats.currentMeetingSubject = currentBookings.value[curretMeetingIndex].subject;
        }

        string GetCurrenMeetinStartEndTime(Bookings currentBookings)
        {
            int startHour = currentBookings.value[curretMeetingIndex].start.dateTime.Hour;
            int startMinute = currentBookings.value[curretMeetingIndex].start.dateTime.Minute;
            int endHour = currentBookings.value[curretMeetingIndex].end.dateTime.Hour;
            int endMinute = currentBookings.value[curretMeetingIndex].end.dateTime.Minute;

            return GetStartEndTimeInString(startHour, startMinute, endHour, endMinute);
        }
        string GetNextMeetinStartEndTime(Bookings currentBookings, int nextBookingIndex)
        {
            int startHour = currentBookings.value[nextBookingIndex].start.dateTime.Hour;
            int startMinute = currentBookings.value[nextBookingIndex].start.dateTime.Minute;
            int endHour = currentBookings.value[nextBookingIndex].end.dateTime.Hour;
            int endMinute = currentBookings.value[nextBookingIndex].end.dateTime.Minute;

            return GetStartEndTimeInString(startHour, startMinute, endHour, endMinute);
        }
        string GetSpecificMeetingStartEndTime(Bookings currentBookings, int meetingIndex)
        {
            int startHour = currentBookings.value[meetingIndex].start.dateTime.Hour;
            int startMinute = currentBookings.value[meetingIndex].start.dateTime.Minute;
            int endHour = currentBookings.value[meetingIndex].end.dateTime.Hour;
            int endMinute = currentBookings.value[meetingIndex].end.dateTime.Minute;

            return GetStartEndTimeInString(startHour, startMinute, endHour, endMinute);
        }
        string GetStartEndTimeInString(int startHour, int startMinute, int endHour, int endMinute)
        {
            string currentMeetingStartEndTimeString;
            if (startHour < 10) currentMeetingStartEndTimeString = "0" + startHour + ":";
            else currentMeetingStartEndTimeString = startHour.ToString() + ":";


            if (startMinute < 10) currentMeetingStartEndTimeString += "0" + startMinute;
            else currentMeetingStartEndTimeString += startMinute.ToString();

            currentMeetingStartEndTimeString += " - ";


            if (endHour < 10) currentMeetingStartEndTimeString += "0" + endHour + ":";
            else currentMeetingStartEndTimeString += endHour.ToString() + ":";


            if (endMinute < 10) currentMeetingStartEndTimeString += "0" + endMinute;
            else currentMeetingStartEndTimeString += endMinute.ToString();

            return currentMeetingStartEndTimeString;
        }

        void FillTimeRemainingInfo(CurrentAndNextMeetingInfo newBookingStats, Bookings currentBookings)
        {
            int timeNow = DateTime.Now.Hour * 60 + DateTime.Now.Minute;

            int timeRemaining = (currentBookings.value[curretMeetingIndex].end.dateTime.Hour * 60 + currentBookings.value[curretMeetingIndex].end.dateTime.Minute) - timeNow;

            if (timeRemaining < 60)
            {
                newBookingStats.currentHoursRemaining = 0;
                newBookingStats.currentMinutesRemaining = timeRemaining;
            }
            else if (timeRemaining == 60)
            {
                newBookingStats.currentHoursRemaining = 1;
                newBookingStats.currentMinutesRemaining = 0;
            }
            else if (timeRemaining > 60)
            {
                newBookingStats.currentHoursRemaining = timeRemaining / 60;
                newBookingStats.currentMinutesRemaining = timeRemaining -(newBookingStats.currentHoursRemaining * 60);
            }
        }

        void  FillNextMeetingInfo(CurrentAndNextMeetingInfo newBookingStats, Bookings currentBookings, int nextBookingIndex)
        {
            newBookingStats.nextMeetingStartEndTime = GetNextMeetinStartEndTime(currentBookings, nextBookingIndex);
            newBookingStats.nextMeetingOrganiser = currentBookings.value[nextBookingIndex].organizer.emailAddress.name;
            newBookingStats.nextMeetingSubject = currentBookings.value[nextBookingIndex].subject;
        }

        int FillTimeUntilInfo(CurrentAndNextMeetingInfo newBookingStats, Bookings currentBookings, int nextMeetingIndex)
        {
            int timeNow = DateTime.Now.Hour * 60 + DateTime.Now.Minute;

            int timeRemaining = (currentBookings.value[nextMeetingIndex].start.dateTime.Hour * 60 + currentBookings.value[nextMeetingIndex].start.dateTime.Minute) - timeNow;

            if (timeRemaining < 60)
            {
                newBookingStats.hoursUntilNextMeeting = 0;
                newBookingStats.minutesUntilNextMeeting = timeRemaining;
            }
            else if (timeRemaining == 60)
            {
                newBookingStats.hoursUntilNextMeeting = 1;
                newBookingStats.minutesUntilNextMeeting = 0;
            }
            else if (timeRemaining > 60)
            {
                newBookingStats.hoursUntilNextMeeting = timeRemaining / 60;
                newBookingStats.minutesUntilNextMeeting = timeRemaining - (newBookingStats.hoursUntilNextMeeting * 60);
            }

            return timeRemaining;
        }

        int GetNextMeetingIndex(int[] startTimes)
        {
            int timeNow = DateTime.Now.Hour * 60 + DateTime.Now.Minute;

            for(int i = 0; i < startTimes.Length;i++)
                if (startTimes[i] > timeNow) 
                    return i;

            return 0;
        }

        string FillDateNowSection()
        {
            var dateNow = DateTime.Now;
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateNow.Month);

            return dateNow.DayOfWeek+ ", "+ monthName+ " " + dateNow.Day+ ", "+ dateNow.Year + " " + dateNow.Hour + ":" + dateNow.Minute;
        }

        public string NewMeetingRequest(string roomID, int requestedTime)
        {
            Bookings currentBookings = JsonConvert.DeserializeObject<Bookings>(FileOperations.loadRoomJson(Int32.Parse(roomID), "Bookings"));

            int[] startTimes = GetAllStartTimes(currentBookings);
            int[] endTimes = GetAllEndTimes(currentBookings);

            string availableStartTime = GetAvaliableStartTime(endTimes, requestedTime);

            int availableStartTimeInMinutes = int.Parse(availableStartTime.Split(':')[0])*60 + int.Parse(availableStartTime.Split(':')[1]);

            List<string> availableEndTimes = GetAvaliableEndTimes(startTimes, availableStartTimeInMinutes, 30, 4);

            NewMeetingInfo newMeetingInfo = new NewMeetingInfo();
            newMeetingInfo.organiser = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(Int32.Parse(roomID), "Core")).roomName;
            newMeetingInfo.startTime = availableStartTime;
            newMeetingInfo.endTimes = availableEndTimes;

            return JsonConvert.SerializeObject(newMeetingInfo, Formatting.Indented);
        }

        public void CreateNewMeeting(string roomID, string meetingSubject, string startTime, string endTime)
        {
            RoomCoreInfo rci = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(int.Parse(roomID), "Core"));
            ConsoleLogger.WriteLine("Creating New Meeting");

            var url = "https://graph.microsoft.com/v1.0/users/" + rci.emailAddress + "/events";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = 10000;
            request.PreAuthenticate = true;
            request.Headers.Add("Authorization", "Bearer " + _cs.microsoftAuthToken);
            request.ContentType = "application/json";

            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var day = DateTime.Now.Day;

            meetingSubject = meetingSubject.Replace("%20", " ");

            try
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json =
                        "{" +
                            "\"subject\":" + "\"" + meetingSubject + "\"," +
                            "\"start\":" + "{" +
                                "\"dateTime\":" + "\"" + year + "-" + month + "-" + day + "T" + startTime + ":00" + "\"," +
                                "\"timeZone\":" + "\"Europe/London\"" +
                            "}," +
                            "\"end\":" + "{" +
                                "\"dateTime\":" + "\"" + year + "-" + month + "-" + day + "T" + endTime + ":00" + "\"," +
                                "\"timeZone\":" + "\"Europe/London\"" +
                            "}" +
                        "}";
                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    //var result = streamReader.ReadToEnd();

                    _cs.GetCalendarBookings();
                }
            }catch(Exception ex)
            {
                ConsoleLogger.WriteLine("Exception in CreateNewMeeting(): " + ex);
            }
        }

        public void EndCurrentMeeting(string roomID)
        {
            RoomCoreInfo rci = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(int.Parse(roomID), "Core"));
            CurrentAndNextMeetingInfo currentMeetingInfo = JsonConvert.DeserializeObject<CurrentAndNextMeetingInfo>(FileOperations.loadRoomJson(int.Parse(roomID), "BookingStats"));
            ConsoleLogger.WriteLine("Ending Meeting");

            var url = "https://graph.microsoft.com/v1.0/users/"+rci.emailAddress +"/events/"+currentMeetingInfo.currentMeetingID;
            ConsoleLogger.WriteLine("rewuest: " + url);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "PATCH";
            request.Timeout = 10000;
            request.PreAuthenticate = true;
            request.Headers.Add("Authorization", "Bearer " + _cs.microsoftAuthToken);
            request.ContentType = "application/json";

            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var day = DateTime.Now.Day;
            var hourInt = DateTime.Now.Hour; // UTC Time

            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo myTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
            ConsoleLogger.WriteLine($"UTC: {utc} --- Daylight Saving Time: {myTimeZone.IsDaylightSavingTime(utc)} ");

            if (myTimeZone.IsDaylightSavingTime(utc))
            {
                ConsoleLogger.WriteLine("-1 hour");
                hourInt -= 1;
            }
            var minuteInt = DateTime.Now.Minute;

            int timeToConvert = (hourInt * 60) + minuteInt;

            try
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json =
                        "{" +
                            "\"end\":" + "{" +
                                "\"dateTime\":" + "\"" + year + "-" + month + "-" + day + "T" + ConvertToStringTime(timeToConvert) + ":00.000Z" + "\"," +
                                "\"timeZone\":" + "\"Europe/London\"" +
                            "}" +
                        "}";
                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    //var result = streamReader.ReadToEnd();

                    _cs.GetCalendarBookings();
                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("Exception in EndCurrentMeeting(): " + ex);
            }
        }

        public string ExtendMeetingRequest(string roomID)
        {
            Bookings currentBookings = JsonConvert.DeserializeObject<Bookings>(FileOperations.loadRoomJson(Int32.Parse(roomID), "Bookings"));
            CurrentAndNextMeetingInfo currentMeetingInfo = JsonConvert.DeserializeObject<CurrentAndNextMeetingInfo>(FileOperations.loadRoomJson(int.Parse(roomID), "BookingStats"));

            ExtendMeetingInfo extedMeetingInfo = new ExtendMeetingInfo();
            extedMeetingInfo.minutesRemaining = currentMeetingInfo.currentMinutesRemaining;

            try
            {
                int[] startTimes = GetAllStartTimes(currentBookings);
                int requestTime = (DateTime.Now.Hour * 60) + DateTime.Now.Minute + currentMeetingInfo.currentMinutesRemaining;

                extedMeetingInfo.extendTimes = GetAvaliableEndTimes(startTimes, requestTime, 15, 8);
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("Exception in ExtendMeetingRequest(): " + ex);
            }

            return JsonConvert.SerializeObject(extedMeetingInfo);
        }

        public void ExtendCurrentMeeting(string roomID, string timeReqested)
        {
            RoomCoreInfo rci = JsonConvert.DeserializeObject<RoomCoreInfo>(FileOperations.loadRoomJson(int.Parse(roomID), "Core"));
            CurrentAndNextMeetingInfo currentMeetingInfo = JsonConvert.DeserializeObject<CurrentAndNextMeetingInfo>(FileOperations.loadRoomJson(int.Parse(roomID), "BookingStats"));
            ConsoleLogger.WriteLine("Exteding Meeting");

            var url = "https://graph.microsoft.com/v1.0/users/" + rci.emailAddress + "/events/" + currentMeetingInfo.currentMeetingID;
            ConsoleLogger.WriteLine("rewuest: " + url);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "PATCH";
            request.Timeout = 10000;
            request.PreAuthenticate = true;
            request.Headers.Add("Authorization", "Bearer " + _cs.microsoftAuthToken);
            request.ContentType = "application/json"; 

            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var day = DateTime.Now.Day;
            var hourInt = int.Parse(timeReqested.Split(':')[0]); // UTC Time

            DateTime utc = DateTime.UtcNow;
            TimeZoneInfo myTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
            ConsoleLogger.WriteLine($"UTC: {utc} --- Daylight Saving Time: {myTimeZone.IsDaylightSavingTime(utc)} ");

            if (myTimeZone.IsDaylightSavingTime(utc))
            {
                ConsoleLogger.WriteLine("-1 hour");
                hourInt -= 1;
            }
            var minuteInt = int.Parse(timeReqested.Split(':')[1]);

            int timeToConvert = (hourInt * 60) + minuteInt;

            try
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json =
                        "{" +
                            "\"end\":" + "{" +
                                "\"dateTime\":" + "\"" + year + "-" + month + "-" + day + "T" + ConvertToStringTime(timeToConvert) + ":00.000Z" + "\"," +
                                "\"timeZone\":" + "\"Europe/London\"" +
                            "}" +
                        "}";
                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    //var result = streamReader.ReadToEnd();

                    _cs.GetCalendarBookings();
                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.WriteLine("Exception in EndCurrentMeeting(): " + ex);
            }
        }

        string GetAvaliableStartTime(int[] endTimes, int requestedTime)
        {
            //Check for end times on requested time block
            foreach (int endTime in endTimes)
                if (endTime > requestedTime && endTime < (requestedTime + 30))
                    return ConvertToStringTime(endTime);

            return ConvertToStringTime(requestedTime);
        }

        List<string> GetAvaliableEndTimes(int[] startTimes, int requestedTime, int interval, int timeSlotsNeeded)
        {
            List<int> availableEndTimes = new List<int>();

            try
            {
                int nextBlockTime = interval;

                for (int i = 0; i < timeSlotsNeeded; i++)
                {
                    foreach (int startTime in startTimes)
                        if (startTime >= requestedTime && startTime < (requestedTime + nextBlockTime))
                        {
                            if(!availableEndTimes.Contains(startTime))
                                availableEndTimes.Add(startTime);
                            return ConvertToStringList(availableEndTimes);
                        }

                    if (requestedTime + nextBlockTime < 1440) // if available end time is not greater than 24:00
                    {
                        availableEndTimes.Add(requestedTime + nextBlockTime);
                        nextBlockTime = nextBlockTime + interval;
                    }
                    else
                        return ConvertToStringList(availableEndTimes);
                }

            }
            catch(Exception e)
            {
                ConsoleLogger.WriteLine("Exception in BookingManager.GetAvailableEndTimes(): " + e);
            }

            return ConvertToStringList(availableEndTimes);
        }

        List<string> ConvertToStringList(List<int> availableEndTimes)
        {
            List<string> result = new List<string>();

            try
            {
                foreach (int endTime in availableEndTimes)
                    result.Add(ConvertToStringTime(endTime));
            }catch(Exception e)
            {
                ConsoleLogger.WriteLine("Exception in ConvertToStringList(): " + e);
            }

            return result;
        }

        string ConvertToStringTime(int newStartTime)
        {
            int hours = newStartTime / 60;
            int minutes = newStartTime % 60;

            string hoursText;
            if (hours < 10) hoursText = "0" + hours;
            else hoursText = hours.ToString();

            string minutesText;
            if (minutes < 10) minutesText = "0" + minutes;
            else minutesText = minutes.ToString();

            return hoursText + ":" + minutesText;
        }

    }
}
