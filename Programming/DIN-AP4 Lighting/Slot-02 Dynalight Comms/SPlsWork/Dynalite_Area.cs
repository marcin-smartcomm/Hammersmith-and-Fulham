using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using Crestron;
using Crestron.Logos.SplusLibrary;
using Crestron.Logos.SplusObjects;
using Crestron.SimplSharp;

namespace UserModule_DYNALITE_AREA
{
    public class UserModuleClass_DYNALITE_AREA : SplusObject
    {
        static CCriticalSection g_criticalSection = new CCriticalSection();
        
        
        
        
        
        
        
        Crestron.Logos.SplusObjects.DigitalInput CLASSIC__DOLLAR__;
        Crestron.Logos.SplusObjects.DigitalInput PROGRAM__DOLLAR__;
        Crestron.Logos.SplusObjects.StringInput RX__DOLLAR__;
        Crestron.Logos.SplusObjects.AnalogInput AREA__DOLLAR__;
        Crestron.Logos.SplusObjects.AnalogInput FADE__DOLLAR__;
        InOutArray<Crestron.Logos.SplusObjects.DigitalInput> PRESET__DOLLAR__;
        InOutArray<Crestron.Logos.SplusObjects.AnalogInput> CHANNEL__DOLLAR__;
        Crestron.Logos.SplusObjects.StringOutput TX__DOLLAR__;
        InOutArray<Crestron.Logos.SplusObjects.DigitalOutput> PRESET_FB__DOLLAR__;
        InOutArray<Crestron.Logos.SplusObjects.AnalogOutput> CHANNEL_FB__DOLLAR__;
        object PRESET__DOLLAR___OnPush_0 ( Object __EventInfo__ )
        
            { 
            Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
            try
            {
                SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
                ushort X = 0;
                
                
                __context__.SourceCodeLine = 147;
                X = (ushort) ( Functions.GetLastModifiedArrayIndex( __SignalEventArg__ ) ) ; 
                __context__.SourceCodeLine = 148;
                _SplusNVRAM.CURRENTPRESET = (ushort) ( X ) ; 
                __context__.SourceCodeLine = 149;
                if ( Functions.TestForTrue  ( ( CLASSIC__DOLLAR__  .Value)  ) ) 
                    { 
                    __context__.SourceCodeLine = 151;
                    _SplusNVRAM.BANK = (ushort) ( ((X - 1) / 8) ) ; 
                    __context__.SourceCodeLine = 152;
                    _SplusNVRAM.OPCODE = (ushort) ( (X - (_SplusNVRAM.BANK * 8)) ) ; 
                    __context__.SourceCodeLine = 153;
                    
                        {
                        int __SPLS_TMPVAR__SWTCH_1__ = ((int)_SplusNVRAM.OPCODE);
                        
                            { 
                            if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_1__ == ( 1) ) ) ) 
                                {
                                __context__.SourceCodeLine = 156;
                                _SplusNVRAM.OPCODE = (ushort) ( 0 ) ; 
                                }
                            
                            else if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_1__ == ( 2) ) ) ) 
                                {
                                __context__.SourceCodeLine = 158;
                                _SplusNVRAM.OPCODE = (ushort) ( 1 ) ; 
                                }
                            
                            else if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_1__ == ( 3) ) ) ) 
                                {
                                __context__.SourceCodeLine = 160;
                                _SplusNVRAM.OPCODE = (ushort) ( 2 ) ; 
                                }
                            
                            else if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_1__ == ( 4) ) ) ) 
                                {
                                __context__.SourceCodeLine = 162;
                                _SplusNVRAM.OPCODE = (ushort) ( 3 ) ; 
                                }
                            
                            else if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_1__ == ( 5) ) ) ) 
                                {
                                __context__.SourceCodeLine = 164;
                                _SplusNVRAM.OPCODE = (ushort) ( 10 ) ; 
                                }
                            
                            else if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_1__ == ( 6) ) ) ) 
                                {
                                __context__.SourceCodeLine = 166;
                                _SplusNVRAM.OPCODE = (ushort) ( 11 ) ; 
                                }
                            
                            else if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_1__ == ( 7) ) ) ) 
                                {
                                __context__.SourceCodeLine = 168;
                                _SplusNVRAM.OPCODE = (ushort) ( 12 ) ; 
                                }
                            
                            else if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_1__ == ( 8) ) ) ) 
                                {
                                __context__.SourceCodeLine = 170;
                                _SplusNVRAM.OPCODE = (ushort) ( 13 ) ; 
                                }
                            
                            } 
                            
                        }
                        
                    
                    __context__.SourceCodeLine = 172;
                    MakeString ( TX__DOLLAR__ , "\u001C{0}\u0064{1}\u0000{2}{3}", Functions.Chr (  (int) ( AREA__DOLLAR__  .UshortValue ) ) , Functions.Chr (  (int) ( _SplusNVRAM.OPCODE ) ) , Functions.Chr (  (int) ( _SplusNVRAM.BANK ) ) , Functions.Chr (  (int) ( _SplusNVRAM.JOIN ) ) ) ; 
                    } 
                
                else 
                    { 
                    __context__.SourceCodeLine = 176;
                    MakeString ( TX__DOLLAR__ , "\u001C{0}{1}\u0065\u0064\u0000{2}", Functions.Chr (  (int) ( AREA__DOLLAR__  .UshortValue ) ) , Functions.Chr (  (int) ( (X - 1) ) ) , Functions.Chr (  (int) ( _SplusNVRAM.JOIN ) ) ) ; 
                    } 
                
                
                
            }
            catch(Exception e) { ObjectCatchHandler(e); }
            finally { ObjectFinallyHandler( __SignalEventArg__ ); }
            return this;
            
        }
        
    object PROGRAM__DOLLAR___OnPush_1 ( Object __EventInfo__ )
    
        { 
        Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
        try
        {
            SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
            
            __context__.SourceCodeLine = 182;
            if ( Functions.TestForTrue  ( ( _SplusNVRAM.CURRENTPRESET)  ) ) 
                { 
                __context__.SourceCodeLine = 184;
                MakeString ( TX__DOLLAR__ , "\u001C{0}\u0000\u0008\u0000\u0000{1}", Functions.Chr (  (int) ( AREA__DOLLAR__  .UshortValue ) ) , Functions.Chr (  (int) ( _SplusNVRAM.JOIN ) ) ) ; 
                } 
            
            
            
        }
        catch(Exception e) { ObjectCatchHandler(e); }
        finally { ObjectFinallyHandler( __SignalEventArg__ ); }
        return this;
        
    }
    
object CHANNEL__DOLLAR___OnChange_2 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        ushort X = 0;
        
        ushort RANGE = 0;
        
        ushort FACTOR = 0;
        
        ushort VALUE = 0;
        
        
        __context__.SourceCodeLine = 202;
        X = (ushort) ( Functions.GetLastModifiedArrayIndex( __SignalEventArg__ ) ) ; 
        __context__.SourceCodeLine = 203;
        VALUE = (ushort) ( ((CHANNEL__DOLLAR__[ X ] .UshortValue * 100) / 65535) ) ; 
        __context__.SourceCodeLine = 204;
        VALUE = (ushort) ( (100 - VALUE) ) ; 
        __context__.SourceCodeLine = 205;
        VALUE = (ushort) ( ((VALUE * 255) / 100) ) ; 
        __context__.SourceCodeLine = 206;
        if ( Functions.TestForTrue  ( ( Functions.BoolToInt (VALUE == 0))  ) ) 
            {
            __context__.SourceCodeLine = 206;
            VALUE = (ushort) ( 1 ) ; 
            }
        
        __context__.SourceCodeLine = 207;
        if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( _SplusNVRAM.FADE <= 25 ))  ) ) 
            { 
            __context__.SourceCodeLine = 209;
            RANGE = (ushort) ( 113 ) ; 
            __context__.SourceCodeLine = 210;
            FACTOR = (ushort) ( (_SplusNVRAM.FADE * 10) ) ; 
            } 
        
        else 
            {
            __context__.SourceCodeLine = 212;
            if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( _SplusNVRAM.FADE <= 195 ))  ) ) 
                { 
                __context__.SourceCodeLine = 214;
                RANGE = (ushort) ( 114 ) ; 
                __context__.SourceCodeLine = 215;
                FACTOR = (ushort) ( _SplusNVRAM.FADE ) ; 
                } 
            
            else 
                {
                __context__.SourceCodeLine = 217;
                if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( _SplusNVRAM.FADE <= 1320 ))  ) ) 
                    { 
                    __context__.SourceCodeLine = 219;
                    RANGE = (ushort) ( 115 ) ; 
                    __context__.SourceCodeLine = 220;
                    FACTOR = (ushort) ( (_SplusNVRAM.FADE / 60) ) ; 
                    } 
                
                }
            
            }
        
        __context__.SourceCodeLine = 222;
        if ( Functions.TestForTrue  ( ( CLASSIC__DOLLAR__  .Value)  ) ) 
            { 
            __context__.SourceCodeLine = 224;
            _SplusNVRAM.BANK = (ushort) ( ((X - 1) / 4) ) ; 
            __context__.SourceCodeLine = 225;
            
                {
                int __SPLS_TMPVAR__SWTCH_2__ = ((int)_SplusNVRAM.BANK);
                
                    { 
                    if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_2__ == ( 0) ) ) ) 
                        {
                        __context__.SourceCodeLine = 228;
                        _SplusNVRAM.OFFSET = (ushort) ( 255 ) ; 
                        }
                    
                    else if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_2__ == ( 1) ) ) ) 
                        {
                        __context__.SourceCodeLine = 230;
                        _SplusNVRAM.OFFSET = (ushort) ( 0 ) ; 
                        }
                    
                    else if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_2__ == ( 2) ) ) ) 
                        {
                        __context__.SourceCodeLine = 232;
                        _SplusNVRAM.OFFSET = (ushort) ( 1 ) ; 
                        }
                    
                    else if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_2__ == ( 3) ) ) ) 
                        {
                        __context__.SourceCodeLine = 234;
                        _SplusNVRAM.OFFSET = (ushort) ( 2 ) ; 
                        }
                    
                    } 
                    
                }
                
            
            __context__.SourceCodeLine = 236;
            _SplusNVRAM.OPCODE = (ushort) ( (127 + (X - (_SplusNVRAM.BANK * 4))) ) ; 
            __context__.SourceCodeLine = 237;
            MakeString ( TX__DOLLAR__ , "\u001C{0}{1}{2}{3}\u0064{4}", Functions.Chr (  (int) ( AREA__DOLLAR__  .UshortValue ) ) , Functions.Chr (  (int) ( VALUE ) ) , Functions.Chr (  (int) ( _SplusNVRAM.OPCODE ) ) , Functions.Chr (  (int) ( _SplusNVRAM.OFFSET ) ) , Functions.Chr (  (int) ( _SplusNVRAM.JOIN ) ) ) ; 
            } 
        
        else 
            { 
            __context__.SourceCodeLine = 241;
            MakeString ( TX__DOLLAR__ , "\u001C{0}{1}{2}{3}{4}{5}", Functions.Chr (  (int) ( AREA__DOLLAR__  .UshortValue ) ) , Functions.Chr (  (int) ( (X - 1) ) ) , Functions.Chr (  (int) ( RANGE ) ) , Functions.Chr (  (int) ( VALUE ) ) , Functions.Chr (  (int) ( FACTOR ) ) , Functions.Chr (  (int) ( _SplusNVRAM.JOIN ) ) ) ; 
            } 
        
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object FADE__DOLLAR___OnChange_3 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 247;
        _SplusNVRAM.FADE = (ushort) ( FADE__DOLLAR__  .UshortValue ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object RX__DOLLAR___OnChange_4 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        ushort FACTOR = 0;
        
        ushort VALUE = 0;
        
        
        __context__.SourceCodeLine = 271;
        if ( Functions.TestForTrue  ( ( _SplusNVRAM.XOK)  ) ) 
            { 
            __context__.SourceCodeLine = 273;
            _SplusNVRAM.XOK = (ushort) ( 0 ) ; 
            __context__.SourceCodeLine = 274;
            while ( Functions.TestForTrue  ( ( Functions.Length( RX__DOLLAR__ ))  ) ) 
                { 
                __context__.SourceCodeLine = 276;
                if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( Functions.Length( RX__DOLLAR__ ) >= 8 ))  ) ) 
                    { 
                    __context__.SourceCodeLine = 278;
                    _SplusNVRAM.TEMPSTRING  .UpdateValue ( Functions.Remove ( Functions.Left ( RX__DOLLAR__ ,  (int) ( 8 ) ) , RX__DOLLAR__ )  ) ; 
                    __context__.SourceCodeLine = 279;
                    _SplusNVRAM.OPCODE = (ushort) ( Byte( Functions.Mid( _SplusNVRAM.TEMPSTRING , (int)( 4 ) , (int)( 1 ) ) , (int)( 1 ) ) ) ; 
                    __context__.SourceCodeLine = 280;
                    
                        {
                        int __SPLS_TMPVAR__SWTCH_3__ = ((int)_SplusNVRAM.OPCODE);
                        
                            { 
                            if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_3__ == ( 101) ) ) ) 
                                { 
                                __context__.SourceCodeLine = 284;
                                _SplusNVRAM.PRESET = (ushort) ( (Byte( Functions.Mid( _SplusNVRAM.TEMPSTRING , (int)( 3 ) , (int)( 1 ) ) , (int)( 1 ) ) + 1) ) ; 
                                __context__.SourceCodeLine = 285;
                                ushort __FN_FORSTART_VAL__1 = (ushort) ( 1 ) ;
                                ushort __FN_FOREND_VAL__1 = (ushort)_SplusNVRAM.LASTPRESET; 
                                int __FN_FORSTEP_VAL__1 = (int)1; 
                                for ( _SplusNVRAM.I  = __FN_FORSTART_VAL__1; (__FN_FORSTEP_VAL__1 > 0)  ? ( (_SplusNVRAM.I  >= __FN_FORSTART_VAL__1) && (_SplusNVRAM.I  <= __FN_FOREND_VAL__1) ) : ( (_SplusNVRAM.I  <= __FN_FORSTART_VAL__1) && (_SplusNVRAM.I  >= __FN_FOREND_VAL__1) ) ; _SplusNVRAM.I  += (ushort)__FN_FORSTEP_VAL__1) 
                                    { 
                                    __context__.SourceCodeLine = 287;
                                    PRESET_FB__DOLLAR__ [ _SplusNVRAM.I]  .Value = (ushort) ( 0 ) ; 
                                    __context__.SourceCodeLine = 285;
                                    } 
                                
                                __context__.SourceCodeLine = 289;
                                PRESET_FB__DOLLAR__ [ _SplusNVRAM.PRESET]  .Value = (ushort) ( 1 ) ; 
                                __context__.SourceCodeLine = 290;
                                CreateWait ( "__SPLS_TMPVAR__WAITLABEL_9__" , 20 , __SPLS_TMPVAR__WAITLABEL_9___Callback ) ;
                                } 
                            
                            else if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_3__ == (  (int) ( 96 ) ) ) ) ) 
                                { 
                                __context__.SourceCodeLine = 302;
                                _SplusNVRAM.CHANNEL = (ushort) ( (Byte( Functions.Mid( _SplusNVRAM.TEMPSTRING , (int)( 3 ) , (int)( 1 ) ) , (int)( 1 ) ) + 1) ) ; 
                                __context__.SourceCodeLine = 303;
                                VALUE = (ushort) ( Byte( Functions.Mid( _SplusNVRAM.TEMPSTRING , (int)( 5 ) , (int)( 1 ) ) , (int)( 1 ) ) ) ; 
                                __context__.SourceCodeLine = 304;
                                VALUE = (ushort) ( ((VALUE * 100) / 255) ) ; 
                                __context__.SourceCodeLine = 305;
                                VALUE = (ushort) ( (100 - VALUE) ) ; 
                                __context__.SourceCodeLine = 306;
                                VALUE = (ushort) ( ((VALUE * 65535) / 100) ) ; 
                                __context__.SourceCodeLine = 307;
                                CHANNEL_FB__DOLLAR__ [ _SplusNVRAM.CHANNEL]  .Value = (ushort) ( VALUE ) ; 
                                } 
                            
                            } 
                            
                        }
                        
                    
                    __context__.SourceCodeLine = 310;
                    if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( (Functions.TestForTrue ( Functions.BoolToInt ( _SplusNVRAM.OPCODE >= 0 ) ) && Functions.TestForTrue ( Functions.BoolToInt ( _SplusNVRAM.OPCODE <= 3 ) )) ))  ) ) 
                        { 
                        __context__.SourceCodeLine = 313;
                        _SplusNVRAM.BANK = (ushort) ( Byte( Functions.Mid( _SplusNVRAM.TEMPSTRING , (int)( 6 ) , (int)( 1 ) ) , (int)( 1 ) ) ) ; 
                        __context__.SourceCodeLine = 314;
                        _SplusNVRAM.PRESET = (ushort) ( (_SplusNVRAM.OPCODE + 1) ) ; 
                        __context__.SourceCodeLine = 315;
                        _SplusNVRAM.PRESET = (ushort) ( (_SplusNVRAM.PRESET + (8 * _SplusNVRAM.BANK)) ) ; 
                        __context__.SourceCodeLine = 316;
                        ushort __FN_FORSTART_VAL__3 = (ushort) ( 1 ) ;
                        ushort __FN_FOREND_VAL__3 = (ushort)_SplusNVRAM.LASTPRESET; 
                        int __FN_FORSTEP_VAL__3 = (int)1; 
                        for ( _SplusNVRAM.I  = __FN_FORSTART_VAL__3; (__FN_FORSTEP_VAL__3 > 0)  ? ( (_SplusNVRAM.I  >= __FN_FORSTART_VAL__3) && (_SplusNVRAM.I  <= __FN_FOREND_VAL__3) ) : ( (_SplusNVRAM.I  <= __FN_FORSTART_VAL__3) && (_SplusNVRAM.I  >= __FN_FOREND_VAL__3) ) ; _SplusNVRAM.I  += (ushort)__FN_FORSTEP_VAL__3) 
                            { 
                            __context__.SourceCodeLine = 318;
                            PRESET_FB__DOLLAR__ [ _SplusNVRAM.I]  .Value = (ushort) ( 0 ) ; 
                            __context__.SourceCodeLine = 316;
                            } 
                        
                        __context__.SourceCodeLine = 320;
                        PRESET_FB__DOLLAR__ [ _SplusNVRAM.PRESET]  .Value = (ushort) ( 1 ) ; 
                        __context__.SourceCodeLine = 321;
                        CreateWait ( "__SPLS_TMPVAR__WAITLABEL_10__" , 20 , __SPLS_TMPVAR__WAITLABEL_10___Callback ) ;
                        } 
                    
                    __context__.SourceCodeLine = 329;
                    if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( (Functions.TestForTrue ( Functions.BoolToInt ( _SplusNVRAM.OPCODE >= 10 ) ) && Functions.TestForTrue ( Functions.BoolToInt ( _SplusNVRAM.OPCODE <= 13 ) )) ))  ) ) 
                        { 
                        __context__.SourceCodeLine = 332;
                        _SplusNVRAM.BANK = (ushort) ( Byte( Functions.Mid( _SplusNVRAM.TEMPSTRING , (int)( 6 ) , (int)( 1 ) ) , (int)( 1 ) ) ) ; 
                        __context__.SourceCodeLine = 333;
                        _SplusNVRAM.PRESET = (ushort) ( (_SplusNVRAM.OPCODE - 5) ) ; 
                        __context__.SourceCodeLine = 334;
                        _SplusNVRAM.PRESET = (ushort) ( (_SplusNVRAM.PRESET + (8 * _SplusNVRAM.BANK)) ) ; 
                        __context__.SourceCodeLine = 335;
                        ushort __FN_FORSTART_VAL__5 = (ushort) ( 1 ) ;
                        ushort __FN_FOREND_VAL__5 = (ushort)_SplusNVRAM.LASTPRESET; 
                        int __FN_FORSTEP_VAL__5 = (int)1; 
                        for ( _SplusNVRAM.I  = __FN_FORSTART_VAL__5; (__FN_FORSTEP_VAL__5 > 0)  ? ( (_SplusNVRAM.I  >= __FN_FORSTART_VAL__5) && (_SplusNVRAM.I  <= __FN_FOREND_VAL__5) ) : ( (_SplusNVRAM.I  <= __FN_FORSTART_VAL__5) && (_SplusNVRAM.I  >= __FN_FOREND_VAL__5) ) ; _SplusNVRAM.I  += (ushort)__FN_FORSTEP_VAL__5) 
                            { 
                            __context__.SourceCodeLine = 337;
                            PRESET_FB__DOLLAR__ [ _SplusNVRAM.I]  .Value = (ushort) ( 0 ) ; 
                            __context__.SourceCodeLine = 335;
                            } 
                        
                        __context__.SourceCodeLine = 339;
                        PRESET_FB__DOLLAR__ [ _SplusNVRAM.PRESET]  .Value = (ushort) ( 1 ) ; 
                        __context__.SourceCodeLine = 340;
                        CreateWait ( "__SPLS_TMPVAR__WAITLABEL_11__" , 20 , __SPLS_TMPVAR__WAITLABEL_11___Callback ) ;
                        } 
                    
                    __context__.SourceCodeLine = 348;
                    if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( (Functions.TestForTrue ( Functions.BoolToInt ( _SplusNVRAM.OPCODE >= 113 ) ) && Functions.TestForTrue ( Functions.BoolToInt ( _SplusNVRAM.OPCODE <= 115 ) )) ))  ) ) 
                        { 
                        __context__.SourceCodeLine = 351;
                        FACTOR = (ushort) ( Byte( Functions.Mid( _SplusNVRAM.TEMPSTRING , (int)( 6 ) , (int)( 1 ) ) , (int)( 1 ) ) ) ; 
                        __context__.SourceCodeLine = 352;
                        VALUE = (ushort) ( Byte( Functions.Mid( _SplusNVRAM.TEMPSTRING , (int)( 5 ) , (int)( 1 ) ) , (int)( 1 ) ) ) ; 
                        __context__.SourceCodeLine = 353;
                        _SplusNVRAM.CHANNEL = (ushort) ( Byte( Functions.Mid( _SplusNVRAM.TEMPSTRING , (int)( 3 ) , (int)( 1 ) ) , (int)( 1 ) ) ) ; 
                        __context__.SourceCodeLine = 354;
                        VALUE = (ushort) ( ((VALUE * 100) / 255) ) ; 
                        __context__.SourceCodeLine = 355;
                        VALUE = (ushort) ( (100 - VALUE) ) ; 
                        __context__.SourceCodeLine = 356;
                        VALUE = (ushort) ( ((VALUE * 65535) / 100) ) ; 
                        __context__.SourceCodeLine = 357;
                        
                            {
                            int __SPLS_TMPVAR__SWTCH_4__ = ((int)_SplusNVRAM.OPCODE);
                            
                                { 
                                if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_4__ == ( 113) ) ) ) 
                                    { 
                                    __context__.SourceCodeLine = 361;
                                    CHANNEL_FB__DOLLAR__ [ (_SplusNVRAM.CHANNEL + 1)]  .Value = (ushort) ( VALUE ) ; 
                                    } 
                                
                                else if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_4__ == ( 114) ) ) ) 
                                    { 
                                    __context__.SourceCodeLine = 365;
                                    if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( FACTOR <= 195 ))  ) ) 
                                        { 
                                        __context__.SourceCodeLine = 367;
                                        CHANNEL_FB__DOLLAR__ [ (_SplusNVRAM.CHANNEL + 1)]  .Value = (ushort) ( VALUE ) ; 
                                        } 
                                    
                                    } 
                                
                                else if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_4__ == ( 115) ) ) ) 
                                    { 
                                    __context__.SourceCodeLine = 372;
                                    if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( FACTOR <= 22 ))  ) ) 
                                        { 
                                        __context__.SourceCodeLine = 374;
                                        CHANNEL_FB__DOLLAR__ [ (_SplusNVRAM.CHANNEL + 1)]  .Value = (ushort) ( VALUE ) ; 
                                        } 
                                    
                                    } 
                                
                                } 
                                
                            }
                            
                        
                        } 
                    
                    __context__.SourceCodeLine = 379;
                    if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( (Functions.TestForTrue ( Functions.BoolToInt ( _SplusNVRAM.OPCODE >= 128 ) ) && Functions.TestForTrue ( Functions.BoolToInt ( _SplusNVRAM.OPCODE <= 131 ) )) ))  ) ) 
                        { 
                        __context__.SourceCodeLine = 382;
                        VALUE = (ushort) ( Byte( Functions.Mid( _SplusNVRAM.TEMPSTRING , (int)( 3 ) , (int)( 1 ) ) , (int)( 1 ) ) ) ; 
                        __context__.SourceCodeLine = 383;
                        VALUE = (ushort) ( ((VALUE * 100) / 255) ) ; 
                        __context__.SourceCodeLine = 384;
                        VALUE = (ushort) ( (100 - VALUE) ) ; 
                        __context__.SourceCodeLine = 385;
                        VALUE = (ushort) ( ((VALUE * 65535) / 100) ) ; 
                        __context__.SourceCodeLine = 386;
                        _SplusNVRAM.OPCODE = (ushort) ( Byte( Functions.Mid( _SplusNVRAM.TEMPSTRING , (int)( 4 ) , (int)( 1 ) ) , (int)( 1 ) ) ) ; 
                        __context__.SourceCodeLine = 387;
                        _SplusNVRAM.OPCODE = (ushort) ( (_SplusNVRAM.OPCODE - 127) ) ; 
                        __context__.SourceCodeLine = 388;
                        _SplusNVRAM.OFFSET = (ushort) ( Byte( Functions.Mid( _SplusNVRAM.TEMPSTRING , (int)( 5 ) , (int)( 1 ) ) , (int)( 1 ) ) ) ; 
                        __context__.SourceCodeLine = 389;
                        
                            {
                            int __SPLS_TMPVAR__SWTCH_5__ = ((int)_SplusNVRAM.OFFSET);
                            
                                { 
                                if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_5__ == ( 255) ) ) ) 
                                    {
                                    __context__.SourceCodeLine = 392;
                                    _SplusNVRAM.CHANNEL = (ushort) ( _SplusNVRAM.OPCODE ) ; 
                                    }
                                
                                else if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_5__ == ( 0) ) ) ) 
                                    {
                                    __context__.SourceCodeLine = 394;
                                    _SplusNVRAM.CHANNEL = (ushort) ( (_SplusNVRAM.OPCODE + 4) ) ; 
                                    }
                                
                                else if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_5__ == ( 1) ) ) ) 
                                    {
                                    __context__.SourceCodeLine = 396;
                                    _SplusNVRAM.CHANNEL = (ushort) ( (_SplusNVRAM.OPCODE + 8) ) ; 
                                    }
                                
                                else if  ( Functions.TestForTrue  (  ( __SPLS_TMPVAR__SWTCH_5__ == ( 2) ) ) ) 
                                    {
                                    __context__.SourceCodeLine = 398;
                                    _SplusNVRAM.CHANNEL = (ushort) ( (_SplusNVRAM.OPCODE + 12) ) ; 
                                    }
                                
                                } 
                                
                            }
                            
                        
                        __context__.SourceCodeLine = 400;
                        CHANNEL_FB__DOLLAR__ [ _SplusNVRAM.CHANNEL]  .Value = (ushort) ( VALUE ) ; 
                        } 
                    
                    __context__.SourceCodeLine = 402;
                    Functions.ClearBuffer ( _SplusNVRAM.TEMPSTRING ) ; 
                    } 
                
                __context__.SourceCodeLine = 274;
                } 
            
            __context__.SourceCodeLine = 405;
            _SplusNVRAM.XOK = (ushort) ( 1 ) ; 
            } 
        
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

public void __SPLS_TMPVAR__WAITLABEL_9___CallbackFn( object stateInfo )
{

    try
    {
        Wait __LocalWait__ = (Wait)stateInfo;
        SplusExecutionContext __context__ = SplusThreadStartCode(__LocalWait__);
        __LocalWait__.RemoveFromList();
        
            
            __context__.SourceCodeLine = 292;
            ushort __FN_FORSTART_VAL__2 = (ushort) ( 0 ) ;
            ushort __FN_FOREND_VAL__2 = (ushort)(_SplusNVRAM.LASTCHANNEL - 1); 
            int __FN_FORSTEP_VAL__2 = (int)1; 
            for ( _SplusNVRAM.I  = __FN_FORSTART_VAL__2; (__FN_FORSTEP_VAL__2 > 0)  ? ( (_SplusNVRAM.I  >= __FN_FORSTART_VAL__2) && (_SplusNVRAM.I  <= __FN_FOREND_VAL__2) ) : ( (_SplusNVRAM.I  <= __FN_FORSTART_VAL__2) && (_SplusNVRAM.I  >= __FN_FOREND_VAL__2) ) ; _SplusNVRAM.I  += (ushort)__FN_FORSTEP_VAL__2) 
                { 
                __context__.SourceCodeLine = 294;
                MakeString ( TX__DOLLAR__ , "\u001C{0}{1}\u0061\u0000\u0000{2}", Functions.Chr (  (int) ( AREA__DOLLAR__  .UshortValue ) ) , Functions.Chr (  (int) ( _SplusNVRAM.I ) ) , Functions.Chr (  (int) ( _SplusNVRAM.JOIN ) ) ) ; 
                __context__.SourceCodeLine = 292;
                } 
            
            
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler(); }
    
}

public void __SPLS_TMPVAR__WAITLABEL_10___CallbackFn( object stateInfo )
{

    try
    {
        Wait __LocalWait__ = (Wait)stateInfo;
        SplusExecutionContext __context__ = SplusThreadStartCode(__LocalWait__);
        __LocalWait__.RemoveFromList();
        
            
            __context__.SourceCodeLine = 323;
            ushort __FN_FORSTART_VAL__4 = (ushort) ( 0 ) ;
            ushort __FN_FOREND_VAL__4 = (ushort)(_SplusNVRAM.LASTCHANNEL - 1); 
            int __FN_FORSTEP_VAL__4 = (int)1; 
            for ( _SplusNVRAM.I  = __FN_FORSTART_VAL__4; (__FN_FORSTEP_VAL__4 > 0)  ? ( (_SplusNVRAM.I  >= __FN_FORSTART_VAL__4) && (_SplusNVRAM.I  <= __FN_FOREND_VAL__4) ) : ( (_SplusNVRAM.I  <= __FN_FORSTART_VAL__4) && (_SplusNVRAM.I  >= __FN_FOREND_VAL__4) ) ; _SplusNVRAM.I  += (ushort)__FN_FORSTEP_VAL__4) 
                { 
                __context__.SourceCodeLine = 325;
                MakeString ( TX__DOLLAR__ , "\u001C{0}{1}\u0061\u0000\u0000{2}", Functions.Chr (  (int) ( AREA__DOLLAR__  .UshortValue ) ) , Functions.Chr (  (int) ( _SplusNVRAM.I ) ) , Functions.Chr (  (int) ( _SplusNVRAM.JOIN ) ) ) ; 
                __context__.SourceCodeLine = 323;
                } 
            
            
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler(); }
    
}

public void __SPLS_TMPVAR__WAITLABEL_11___CallbackFn( object stateInfo )
{

    try
    {
        Wait __LocalWait__ = (Wait)stateInfo;
        SplusExecutionContext __context__ = SplusThreadStartCode(__LocalWait__);
        __LocalWait__.RemoveFromList();
        
            
            __context__.SourceCodeLine = 342;
            ushort __FN_FORSTART_VAL__6 = (ushort) ( 0 ) ;
            ushort __FN_FOREND_VAL__6 = (ushort)(_SplusNVRAM.LASTCHANNEL - 1); 
            int __FN_FORSTEP_VAL__6 = (int)1; 
            for ( _SplusNVRAM.I  = __FN_FORSTART_VAL__6; (__FN_FORSTEP_VAL__6 > 0)  ? ( (_SplusNVRAM.I  >= __FN_FORSTART_VAL__6) && (_SplusNVRAM.I  <= __FN_FOREND_VAL__6) ) : ( (_SplusNVRAM.I  <= __FN_FORSTART_VAL__6) && (_SplusNVRAM.I  >= __FN_FOREND_VAL__6) ) ; _SplusNVRAM.I  += (ushort)__FN_FORSTEP_VAL__6) 
                { 
                __context__.SourceCodeLine = 344;
                MakeString ( TX__DOLLAR__ , "\u001C{0}{1}\u0061\u0000\u0000{2}", Functions.Chr (  (int) ( AREA__DOLLAR__  .UshortValue ) ) , Functions.Chr (  (int) ( _SplusNVRAM.I ) ) , Functions.Chr (  (int) ( _SplusNVRAM.JOIN ) ) ) ; 
                __context__.SourceCodeLine = 342;
                } 
            
            
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler(); }
    
}

public override object FunctionMain (  object __obj__ ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusFunctionMainStartCode();
        
        __context__.SourceCodeLine = 424;
        _SplusNVRAM.XOK = (ushort) ( 1 ) ; 
        __context__.SourceCodeLine = 425;
        _SplusNVRAM.JOIN = (ushort) ( 255 ) ; 
        __context__.SourceCodeLine = 426;
        ushort __FN_FORSTART_VAL__1 = (ushort) ( 170 ) ;
        ushort __FN_FOREND_VAL__1 = (ushort)1; 
        int __FN_FORSTEP_VAL__1 = (int)Functions.ToLongInteger( -( 1 ) ); 
        for ( _SplusNVRAM.LASTPRESET  = __FN_FORSTART_VAL__1; (__FN_FORSTEP_VAL__1 > 0)  ? ( (_SplusNVRAM.LASTPRESET  >= __FN_FORSTART_VAL__1) && (_SplusNVRAM.LASTPRESET  <= __FN_FOREND_VAL__1) ) : ( (_SplusNVRAM.LASTPRESET  <= __FN_FORSTART_VAL__1) && (_SplusNVRAM.LASTPRESET  >= __FN_FOREND_VAL__1) ) ; _SplusNVRAM.LASTPRESET  += (ushort)__FN_FORSTEP_VAL__1) 
            { 
            __context__.SourceCodeLine = 428;
            if ( Functions.TestForTrue  ( ( IsSignalDefined( PRESET__DOLLAR__[ _SplusNVRAM.LASTPRESET ] ))  ) ) 
                { 
                __context__.SourceCodeLine = 430;
                break ; 
                } 
            
            __context__.SourceCodeLine = 426;
            } 
        
        __context__.SourceCodeLine = 433;
        ushort __FN_FORSTART_VAL__2 = (ushort) ( 255 ) ;
        ushort __FN_FOREND_VAL__2 = (ushort)1; 
        int __FN_FORSTEP_VAL__2 = (int)Functions.ToLongInteger( -( 1 ) ); 
        for ( _SplusNVRAM.LASTCHANNEL  = __FN_FORSTART_VAL__2; (__FN_FORSTEP_VAL__2 > 0)  ? ( (_SplusNVRAM.LASTCHANNEL  >= __FN_FORSTART_VAL__2) && (_SplusNVRAM.LASTCHANNEL  <= __FN_FOREND_VAL__2) ) : ( (_SplusNVRAM.LASTCHANNEL  <= __FN_FORSTART_VAL__2) && (_SplusNVRAM.LASTCHANNEL  >= __FN_FOREND_VAL__2) ) ; _SplusNVRAM.LASTCHANNEL  += (ushort)__FN_FORSTEP_VAL__2) 
            { 
            __context__.SourceCodeLine = 435;
            if ( Functions.TestForTrue  ( ( IsSignalDefined( CHANNEL__DOLLAR__[ _SplusNVRAM.LASTCHANNEL ] ))  ) ) 
                { 
                __context__.SourceCodeLine = 437;
                break ; 
                } 
            
            __context__.SourceCodeLine = 433;
            } 
        
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler(); }
    return __obj__;
    }
    

public override void LogosSplusInitialize()
{
    _SplusNVRAM = new SplusNVRAM( this );
    _SplusNVRAM.TEMPSTRING  = new CrestronString( Crestron.Logos.SplusObjects.CrestronStringEncoding.eEncodingASCII, 8, this );
    
    CLASSIC__DOLLAR__ = new Crestron.Logos.SplusObjects.DigitalInput( CLASSIC__DOLLAR____DigitalInput__, this );
    m_DigitalInputList.Add( CLASSIC__DOLLAR____DigitalInput__, CLASSIC__DOLLAR__ );
    
    PROGRAM__DOLLAR__ = new Crestron.Logos.SplusObjects.DigitalInput( PROGRAM__DOLLAR____DigitalInput__, this );
    m_DigitalInputList.Add( PROGRAM__DOLLAR____DigitalInput__, PROGRAM__DOLLAR__ );
    
    PRESET__DOLLAR__ = new InOutArray<DigitalInput>( 170, this );
    for( uint i = 0; i < 170; i++ )
    {
        PRESET__DOLLAR__[i+1] = new Crestron.Logos.SplusObjects.DigitalInput( PRESET__DOLLAR____DigitalInput__ + i, PRESET__DOLLAR____DigitalInput__, this );
        m_DigitalInputList.Add( PRESET__DOLLAR____DigitalInput__ + i, PRESET__DOLLAR__[i+1] );
    }
    
    PRESET_FB__DOLLAR__ = new InOutArray<DigitalOutput>( 170, this );
    for( uint i = 0; i < 170; i++ )
    {
        PRESET_FB__DOLLAR__[i+1] = new Crestron.Logos.SplusObjects.DigitalOutput( PRESET_FB__DOLLAR____DigitalOutput__ + i, this );
        m_DigitalOutputList.Add( PRESET_FB__DOLLAR____DigitalOutput__ + i, PRESET_FB__DOLLAR__[i+1] );
    }
    
    AREA__DOLLAR__ = new Crestron.Logos.SplusObjects.AnalogInput( AREA__DOLLAR____AnalogSerialInput__, this );
    m_AnalogInputList.Add( AREA__DOLLAR____AnalogSerialInput__, AREA__DOLLAR__ );
    
    FADE__DOLLAR__ = new Crestron.Logos.SplusObjects.AnalogInput( FADE__DOLLAR____AnalogSerialInput__, this );
    m_AnalogInputList.Add( FADE__DOLLAR____AnalogSerialInput__, FADE__DOLLAR__ );
    
    CHANNEL__DOLLAR__ = new InOutArray<AnalogInput>( 255, this );
    for( uint i = 0; i < 255; i++ )
    {
        CHANNEL__DOLLAR__[i+1] = new Crestron.Logos.SplusObjects.AnalogInput( CHANNEL__DOLLAR____AnalogSerialInput__ + i, CHANNEL__DOLLAR____AnalogSerialInput__, this );
        m_AnalogInputList.Add( CHANNEL__DOLLAR____AnalogSerialInput__ + i, CHANNEL__DOLLAR__[i+1] );
    }
    
    CHANNEL_FB__DOLLAR__ = new InOutArray<AnalogOutput>( 255, this );
    for( uint i = 0; i < 255; i++ )
    {
        CHANNEL_FB__DOLLAR__[i+1] = new Crestron.Logos.SplusObjects.AnalogOutput( CHANNEL_FB__DOLLAR____AnalogSerialOutput__ + i, this );
        m_AnalogOutputList.Add( CHANNEL_FB__DOLLAR____AnalogSerialOutput__ + i, CHANNEL_FB__DOLLAR__[i+1] );
    }
    
    RX__DOLLAR__ = new Crestron.Logos.SplusObjects.StringInput( RX__DOLLAR____AnalogSerialInput__, 2000, this );
    m_StringInputList.Add( RX__DOLLAR____AnalogSerialInput__, RX__DOLLAR__ );
    
    TX__DOLLAR__ = new Crestron.Logos.SplusObjects.StringOutput( TX__DOLLAR____AnalogSerialOutput__, this );
    m_StringOutputList.Add( TX__DOLLAR____AnalogSerialOutput__, TX__DOLLAR__ );
    
    __SPLS_TMPVAR__WAITLABEL_9___Callback = new WaitFunction( __SPLS_TMPVAR__WAITLABEL_9___CallbackFn );
    __SPLS_TMPVAR__WAITLABEL_10___Callback = new WaitFunction( __SPLS_TMPVAR__WAITLABEL_10___CallbackFn );
    __SPLS_TMPVAR__WAITLABEL_11___Callback = new WaitFunction( __SPLS_TMPVAR__WAITLABEL_11___CallbackFn );
    
    for( uint i = 0; i < 170; i++ )
        PRESET__DOLLAR__[i+1].OnDigitalPush.Add( new InputChangeHandlerWrapper( PRESET__DOLLAR___OnPush_0, false ) );
        
    PROGRAM__DOLLAR__.OnDigitalPush.Add( new InputChangeHandlerWrapper( PROGRAM__DOLLAR___OnPush_1, false ) );
    for( uint i = 0; i < 255; i++ )
        CHANNEL__DOLLAR__[i+1].OnAnalogChange.Add( new InputChangeHandlerWrapper( CHANNEL__DOLLAR___OnChange_2, false ) );
        
    FADE__DOLLAR__.OnAnalogChange.Add( new InputChangeHandlerWrapper( FADE__DOLLAR___OnChange_3, false ) );
    RX__DOLLAR__.OnSerialChange.Add( new InputChangeHandlerWrapper( RX__DOLLAR___OnChange_4, false ) );
    
    _SplusNVRAM.PopulateCustomAttributeList( true );
    
    NVRAM = _SplusNVRAM;
    
}

public override void LogosSimplSharpInitialize()
{
    
    
}

public UserModuleClass_DYNALITE_AREA ( string InstanceName, string ReferenceID, Crestron.Logos.SplusObjects.CrestronStringEncoding nEncodingType ) : base( InstanceName, ReferenceID, nEncodingType ) {}


private WaitFunction __SPLS_TMPVAR__WAITLABEL_9___Callback;
private WaitFunction __SPLS_TMPVAR__WAITLABEL_10___Callback;
private WaitFunction __SPLS_TMPVAR__WAITLABEL_11___Callback;


const uint CLASSIC__DOLLAR____DigitalInput__ = 0;
const uint PROGRAM__DOLLAR____DigitalInput__ = 1;
const uint RX__DOLLAR____AnalogSerialInput__ = 0;
const uint AREA__DOLLAR____AnalogSerialInput__ = 1;
const uint FADE__DOLLAR____AnalogSerialInput__ = 2;
const uint PRESET__DOLLAR____DigitalInput__ = 2;
const uint CHANNEL__DOLLAR____AnalogSerialInput__ = 3;
const uint TX__DOLLAR____AnalogSerialOutput__ = 0;
const uint PRESET_FB__DOLLAR____DigitalOutput__ = 0;
const uint CHANNEL_FB__DOLLAR____AnalogSerialOutput__ = 1;

[SplusStructAttribute(-1, true, false)]
public class SplusNVRAM : SplusStructureBase
{

    public SplusNVRAM( SplusObject __caller__ ) : base( __caller__ ) {}
    
    [SplusStructAttribute(0, false, true)]
            public ushort LASTPRESET = 0;
            [SplusStructAttribute(1, false, true)]
            public ushort LASTCHANNEL = 0;
            [SplusStructAttribute(2, false, true)]
            public ushort XOK = 0;
            [SplusStructAttribute(3, false, true)]
            public ushort OPCODE = 0;
            [SplusStructAttribute(4, false, true)]
            public ushort OFFSET = 0;
            [SplusStructAttribute(5, false, true)]
            public ushort PRESET = 0;
            [SplusStructAttribute(6, false, true)]
            public ushort CHANNEL = 0;
            [SplusStructAttribute(7, false, true)]
            public ushort BANK = 0;
            [SplusStructAttribute(8, false, true)]
            public ushort FADE = 0;
            [SplusStructAttribute(9, false, true)]
            public ushort I = 0;
            [SplusStructAttribute(10, false, true)]
            public ushort CURRENTPRESET = 0;
            [SplusStructAttribute(11, false, true)]
            public ushort JOIN = 0;
            [SplusStructAttribute(12, false, true)]
            public CrestronString TEMPSTRING;
            
}

SplusNVRAM _SplusNVRAM = null;

public class __CEvent__ : CEvent
{
    public __CEvent__() {}
    public void Close() { base.Close(); }
    public int Reset() { return base.Reset() ? 1 : 0; }
    public int Set() { return base.Set() ? 1 : 0; }
    public int Wait( int timeOutInMs ) { return base.Wait( timeOutInMs ) ? 1 : 0; }
}
public class __CMutex__ : CMutex
{
    public __CMutex__() {}
    public void Close() { base.Close(); }
    public void ReleaseMutex() { base.ReleaseMutex(); }
    public int WaitForMutex() { return base.WaitForMutex() ? 1 : 0; }
}
 public int IsNull( object obj ){ return (obj == null) ? 1 : 0; }
}


}
