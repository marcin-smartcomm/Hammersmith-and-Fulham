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

namespace UserModule_DYNALITE_RECEIVER
{
    public class UserModuleClass_DYNALITE_RECEIVER : SplusObject
    {
        static CCriticalSection g_criticalSection = new CCriticalSection();
        
        
        
        Crestron.Logos.SplusObjects.BufferInput RX__DOLLAR__;
        Crestron.Logos.SplusObjects.BufferInput FROM_AREAS__DOLLAR__;
        Crestron.Logos.SplusObjects.StringOutput TX__DOLLAR__;
        InOutArray<Crestron.Logos.SplusObjects.StringOutput> TO_AREA__DOLLAR__;
        private void SHOW (  SplusExecutionContext __context__, CrestronString TOONDEZE ) 
            { 
            ushort I = 0;
            
            
            __context__.SourceCodeLine = 99;
            Print( "\r\n") ; 
            __context__.SourceCodeLine = 100;
            ushort __FN_FORSTART_VAL__1 = (ushort) ( 1 ) ;
            ushort __FN_FOREND_VAL__1 = (ushort)Functions.Length( TOONDEZE ); 
            int __FN_FORSTEP_VAL__1 = (int)1; 
            for ( I  = __FN_FORSTART_VAL__1; (__FN_FORSTEP_VAL__1 > 0)  ? ( (I  >= __FN_FORSTART_VAL__1) && (I  <= __FN_FOREND_VAL__1) ) : ( (I  <= __FN_FORSTART_VAL__1) && (I  >= __FN_FOREND_VAL__1) ) ; I  += (ushort)__FN_FORSTEP_VAL__1) 
                { 
                __context__.SourceCodeLine = 102;
                Print( "{0:x} ", Byte( Functions.Mid( TOONDEZE , (int)( I ) , (int)( 1 ) ) , (int)( 1 ) )) ; 
                __context__.SourceCodeLine = 100;
                } 
            
            
            }
            
        private CrestronString CHECKED (  SplusExecutionContext __context__, CrestronString TO_CHECK ) 
            { 
            ushort SUM = 0;
            
            ushort I = 0;
            
            
            __context__.SourceCodeLine = 122;
            SUM = (ushort) ( 0 ) ; 
            __context__.SourceCodeLine = 123;
            ushort __FN_FORSTART_VAL__1 = (ushort) ( 1 ) ;
            ushort __FN_FOREND_VAL__1 = (ushort)Functions.Length( TO_CHECK ); 
            int __FN_FORSTEP_VAL__1 = (int)1; 
            for ( I  = __FN_FORSTART_VAL__1; (__FN_FORSTEP_VAL__1 > 0)  ? ( (I  >= __FN_FORSTART_VAL__1) && (I  <= __FN_FOREND_VAL__1) ) : ( (I  <= __FN_FORSTART_VAL__1) && (I  >= __FN_FOREND_VAL__1) ) ; I  += (ushort)__FN_FORSTEP_VAL__1) 
                { 
                __context__.SourceCodeLine = 125;
                SUM = (ushort) ( (SUM + Byte( Functions.Mid( TO_CHECK , (int)( I ) , (int)( 1 ) ) , (int)( 1 ) )) ) ; 
                __context__.SourceCodeLine = 123;
                } 
            
            __context__.SourceCodeLine = 127;
            SUM = (ushort) ( Functions.ToInteger( -( SUM ) ) ) ; 
            __context__.SourceCodeLine = 128;
            return ( Functions.Chr (  (int) ( SUM ) ) ) ; 
            
            }
            
        object RX__DOLLAR___OnChange_0 ( Object __EventInfo__ )
        
            { 
            Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
            try
            {
                SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
                
                __context__.SourceCodeLine = 151;
                if ( Functions.TestForTrue  ( ( _SplusNVRAM.XOK1)  ) ) 
                    { 
                    __context__.SourceCodeLine = 153;
                    _SplusNVRAM.XOK1 = (ushort) ( 0 ) ; 
                    __context__.SourceCodeLine = 154;
                    while ( Functions.TestForTrue  ( ( Functions.Length( RX__DOLLAR__ ))  ) ) 
                        { 
                        __context__.SourceCodeLine = 156;
                        if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( Functions.Length( RX__DOLLAR__ ) >= 8 ))  ) ) 
                            { 
                            __context__.SourceCodeLine = 158;
                            _SplusNVRAM.TEMPSTRING1  .UpdateValue ( Functions.Remove ( Functions.Mid ( RX__DOLLAR__ ,  (int) ( 1 ) ,  (int) ( 8 ) ) , RX__DOLLAR__ )  ) ; 
                            __context__.SourceCodeLine = 159;
                            _SplusNVRAM.AREA = (ushort) ( Byte( Functions.Mid( _SplusNVRAM.TEMPSTRING1 , (int)( 2 ) , (int)( 1 ) ) , (int)( 1 ) ) ) ; 
                            __context__.SourceCodeLine = 160;
                            if ( Functions.TestForTrue  ( ( Functions.BoolToInt (Functions.Mid( _SplusNVRAM.TEMPSTRING1 , (int)( 8 ) , (int)( 1 ) ) == CHECKED( __context__ , Functions.Mid( _SplusNVRAM.TEMPSTRING1 , (int)( 1 ) , (int)( 7 ) ) )))  ) ) 
                                { 
                                __context__.SourceCodeLine = 162;
                                if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( (Functions.TestForTrue ( Functions.BoolToInt ( _SplusNVRAM.AREA <= _SplusNVRAM.LASTAREA ) ) && Functions.TestForTrue ( Functions.BoolToInt ( _SplusNVRAM.AREA > 0 ) )) ))  ) ) 
                                    { 
                                    __context__.SourceCodeLine = 164;
                                    TO_AREA__DOLLAR__ [ _SplusNVRAM.AREA]  .UpdateValue ( _SplusNVRAM.TEMPSTRING1  ) ; 
                                    } 
                                
                                } 
                            
                            __context__.SourceCodeLine = 167;
                            _SplusNVRAM.AREA = (ushort) ( 0 ) ; 
                            __context__.SourceCodeLine = 168;
                            Functions.ClearBuffer ( _SplusNVRAM.TEMPSTRING1 ) ; 
                            } 
                        
                        __context__.SourceCodeLine = 154;
                        } 
                    
                    __context__.SourceCodeLine = 171;
                    _SplusNVRAM.XOK1 = (ushort) ( 1 ) ; 
                    } 
                
                
                
            }
            catch(Exception e) { ObjectCatchHandler(e); }
            finally { ObjectFinallyHandler( __SignalEventArg__ ); }
            return this;
            
        }
        
    object FROM_AREAS__DOLLAR___OnChange_1 ( Object __EventInfo__ )
    
        { 
        Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
        try
        {
            SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
            
            __context__.SourceCodeLine = 177;
            if ( Functions.TestForTrue  ( ( _SplusNVRAM.XOK2)  ) ) 
                { 
                __context__.SourceCodeLine = 179;
                _SplusNVRAM.XOK2 = (ushort) ( 0 ) ; 
                __context__.SourceCodeLine = 180;
                while ( Functions.TestForTrue  ( ( Functions.Length( FROM_AREAS__DOLLAR__ ))  ) ) 
                    { 
                    __context__.SourceCodeLine = 182;
                    CreateWait ( "__SPLS_TMPVAR__WAITLABEL_16__" , 20 , __SPLS_TMPVAR__WAITLABEL_16___Callback ) ;
                    __context__.SourceCodeLine = 180;
                    } 
                
                __context__.SourceCodeLine = 194;
                _SplusNVRAM.XOK2 = (ushort) ( 1 ) ; 
                } 
            
            
            
        }
        catch(Exception e) { ObjectCatchHandler(e); }
        finally { ObjectFinallyHandler( __SignalEventArg__ ); }
        return this;
        
    }
    
public void __SPLS_TMPVAR__WAITLABEL_16___CallbackFn( object stateInfo )
{

    try
    {
        Wait __LocalWait__ = (Wait)stateInfo;
        SplusExecutionContext __context__ = SplusThreadStartCode(__LocalWait__);
        __LocalWait__.RemoveFromList();
        
            
            __context__.SourceCodeLine = 184;
            if ( Functions.TestForTrue  ( ( Functions.BoolToInt ( Functions.Length( FROM_AREAS__DOLLAR__ ) >= 7 ))  ) ) 
                { 
                __context__.SourceCodeLine = 186;
                _SplusNVRAM.TEMPSTRING2  .UpdateValue ( Functions.Remove ( Functions.Left ( FROM_AREAS__DOLLAR__ ,  (int) ( 7 ) ) , FROM_AREAS__DOLLAR__ )  ) ; 
                __context__.SourceCodeLine = 187;
                _SplusNVRAM.CHECKSUM  .UpdateValue ( CHECKED (  __context__ , _SplusNVRAM.TEMPSTRING2)  ) ; 
                __context__.SourceCodeLine = 188;
                _SplusNVRAM.TEMPSTRING2  .UpdateValue ( _SplusNVRAM.TEMPSTRING2 + _SplusNVRAM.CHECKSUM  ) ; 
                __context__.SourceCodeLine = 189;
                MakeString ( TX__DOLLAR__ , "{0}", _SplusNVRAM.TEMPSTRING2 ) ; 
                __context__.SourceCodeLine = 190;
                Functions.ClearBuffer ( _SplusNVRAM.TEMPSTRING2 ) ; 
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
        
        __context__.SourceCodeLine = 213;
        _SplusNVRAM.XOK1 = (ushort) ( 1 ) ; 
        __context__.SourceCodeLine = 214;
        _SplusNVRAM.XOK2 = (ushort) ( 1 ) ; 
        __context__.SourceCodeLine = 215;
        _SplusNVRAM.MARKER1 = (ushort) ( 0 ) ; 
        __context__.SourceCodeLine = 216;
        _SplusNVRAM.MARKER2 = (ushort) ( 0 ) ; 
        __context__.SourceCodeLine = 217;
        ushort __FN_FORSTART_VAL__1 = (ushort) ( 255 ) ;
        ushort __FN_FOREND_VAL__1 = (ushort)1; 
        int __FN_FORSTEP_VAL__1 = (int)Functions.ToLongInteger( -( 1 ) ); 
        for ( _SplusNVRAM.LASTAREA  = __FN_FORSTART_VAL__1; (__FN_FORSTEP_VAL__1 > 0)  ? ( (_SplusNVRAM.LASTAREA  >= __FN_FORSTART_VAL__1) && (_SplusNVRAM.LASTAREA  <= __FN_FOREND_VAL__1) ) : ( (_SplusNVRAM.LASTAREA  <= __FN_FORSTART_VAL__1) && (_SplusNVRAM.LASTAREA  >= __FN_FOREND_VAL__1) ) ; _SplusNVRAM.LASTAREA  += (ushort)__FN_FORSTEP_VAL__1) 
            { 
            __context__.SourceCodeLine = 219;
            if ( Functions.TestForTrue  ( ( IsSignalDefined( TO_AREA__DOLLAR__[ _SplusNVRAM.LASTAREA ] ))  ) ) 
                { 
                __context__.SourceCodeLine = 221;
                break ; 
                } 
            
            __context__.SourceCodeLine = 217;
            } 
        
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler(); }
    return __obj__;
    }
    

public override void LogosSplusInitialize()
{
    SocketInfo __socketinfo__ = new SocketInfo( 1, this );
    InitialParametersClass.ResolveHostName = __socketinfo__.ResolveHostName;
    _SplusNVRAM = new SplusNVRAM( this );
    _SplusNVRAM.TEMPSTRING1  = new CrestronString( Crestron.Logos.SplusObjects.CrestronStringEncoding.eEncodingASCII, 8, this );
    _SplusNVRAM.TEMPSTRING2  = new CrestronString( Crestron.Logos.SplusObjects.CrestronStringEncoding.eEncodingASCII, 8, this );
    _SplusNVRAM.TEMPRX  = new CrestronString( Crestron.Logos.SplusObjects.CrestronStringEncoding.eEncodingASCII, 200, this );
    _SplusNVRAM.CHECKSUM  = new CrestronString( Crestron.Logos.SplusObjects.CrestronStringEncoding.eEncodingASCII, 1, this );
    
    TX__DOLLAR__ = new Crestron.Logos.SplusObjects.StringOutput( TX__DOLLAR____AnalogSerialOutput__, this );
    m_StringOutputList.Add( TX__DOLLAR____AnalogSerialOutput__, TX__DOLLAR__ );
    
    TO_AREA__DOLLAR__ = new InOutArray<StringOutput>( 255, this );
    for( uint i = 0; i < 255; i++ )
    {
        TO_AREA__DOLLAR__[i+1] = new Crestron.Logos.SplusObjects.StringOutput( TO_AREA__DOLLAR____AnalogSerialOutput__ + i, this );
        m_StringOutputList.Add( TO_AREA__DOLLAR____AnalogSerialOutput__ + i, TO_AREA__DOLLAR__[i+1] );
    }
    
    RX__DOLLAR__ = new Crestron.Logos.SplusObjects.BufferInput( RX__DOLLAR____AnalogSerialInput__, 2000, this );
    m_StringInputList.Add( RX__DOLLAR____AnalogSerialInput__, RX__DOLLAR__ );
    
    FROM_AREAS__DOLLAR__ = new Crestron.Logos.SplusObjects.BufferInput( FROM_AREAS__DOLLAR____AnalogSerialInput__, 2000, this );
    m_StringInputList.Add( FROM_AREAS__DOLLAR____AnalogSerialInput__, FROM_AREAS__DOLLAR__ );
    
    __SPLS_TMPVAR__WAITLABEL_16___Callback = new WaitFunction( __SPLS_TMPVAR__WAITLABEL_16___CallbackFn );
    
    RX__DOLLAR__.OnSerialChange.Add( new InputChangeHandlerWrapper( RX__DOLLAR___OnChange_0, false ) );
    FROM_AREAS__DOLLAR__.OnSerialChange.Add( new InputChangeHandlerWrapper( FROM_AREAS__DOLLAR___OnChange_1, false ) );
    
    _SplusNVRAM.PopulateCustomAttributeList( true );
    
    NVRAM = _SplusNVRAM;
    
}

public override void LogosSimplSharpInitialize()
{
    
    
}

public UserModuleClass_DYNALITE_RECEIVER ( string InstanceName, string ReferenceID, Crestron.Logos.SplusObjects.CrestronStringEncoding nEncodingType ) : base( InstanceName, ReferenceID, nEncodingType ) {}


private WaitFunction __SPLS_TMPVAR__WAITLABEL_16___Callback;


const uint RX__DOLLAR____AnalogSerialInput__ = 0;
const uint FROM_AREAS__DOLLAR____AnalogSerialInput__ = 1;
const uint TX__DOLLAR____AnalogSerialOutput__ = 0;
const uint TO_AREA__DOLLAR____AnalogSerialOutput__ = 1;

[SplusStructAttribute(-1, true, false)]
public class SplusNVRAM : SplusStructureBase
{

    public SplusNVRAM( SplusObject __caller__ ) : base( __caller__ ) {}
    
    [SplusStructAttribute(0, false, true)]
            public ushort XOK1 = 0;
            [SplusStructAttribute(1, false, true)]
            public ushort XOK2 = 0;
            [SplusStructAttribute(2, false, true)]
            public ushort MARKER1 = 0;
            [SplusStructAttribute(3, false, true)]
            public ushort MARKER2 = 0;
            [SplusStructAttribute(4, false, true)]
            public ushort AREA = 0;
            [SplusStructAttribute(5, false, true)]
            public ushort LASTAREA = 0;
            [SplusStructAttribute(6, false, true)]
            public CrestronString TEMPSTRING1;
            [SplusStructAttribute(7, false, true)]
            public CrestronString TEMPSTRING2;
            [SplusStructAttribute(8, false, true)]
            public CrestronString TEMPRX;
            [SplusStructAttribute(9, false, true)]
            public CrestronString CHECKSUM;
            
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
