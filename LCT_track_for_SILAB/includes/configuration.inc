Executeable = true;
#SCN.VisibleRoadLength = 3000.0;

	include includes/Schilder.cfg
	include system\DPUTRFDataFull.inc	
	

#----------------------------------------------------------
	DPUKeyboard myKeyboard
	{
		Computer = { OPERATOR };
		Index = 100;  

	};
#----------------------------------------------------------

	
	Connections = 
	{
		VDyn.v_kmh <-> TACHO.v_kmh,
		VDyn.RPM <-> TACHO.rpm
	};

	
	DPUSCNXHedgehogKiller HKLCT
	{
		Computer = {ASS};
		Index = 100;
		Family = "LCT";
		Name1 = "Strecke";
		Name2 = "Marker";
	};


	DPUMMI mmi
 	{
 		Computer = {MOCKUP};
 		Index = 1000;
 		
 		WindowX = 	800;
 		WindowY =	700;
 		WindowH =	50;
 		WindowW =	210;
 		W		=	300;
 		H		=	110;	
 		

 		NumberDisplay Speed
 		{
 			X = 10;
 			Y = 10;
 			W = 280;
 			H = 100;
 			FontColor = (0.98, 0.322, 0);
 			Format = "%.0f km/h"; #Format wird in C-Syntax angegeben
 			Font = "verdanab.ttf";
 		};
 	};
 	
 	 	Connections = 
 	{
 		VDyn.v_kmh <-> mmi.Speed.In
 	};
 
 	
	
	DPUTextFile LCTdaten
	{
		Computer = {DATA};
		Index = 1000;
		Channels = 6;
		Path = "C:\LCT\";
		
		Alias1 = "Streckennummer";
		Alias2 = "Abstand zum rechten Fahrbahnrand";
		Alias3 = "Streckenmeter";
		Alias4 = "Geschwindigkeit in km/h";
		Alias5 = "Lenkradwinkel";
		Alias6 = "Marker";
		
		
	};

	Connections =
	{
		HKLCT.Out1 				<-> LCTdaten.In1,
		SCN.LateralPos 			<-> LCTdaten.In2,
		SCN.s			 		<-> LCTdaten.In3,
		VDyn.v_kmh				<-> LCTdaten.In4,
		Mockup.SteeringWheel 	<-> LCTdaten.In5,
		#HKLCT.Out2 				<-> LCTdaten.In6
		myKeyboard.Key		 <-> LCTdaten.In6
		
	};
	
