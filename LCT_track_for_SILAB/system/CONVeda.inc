			Connections = 
			{
				#
				# Verbindung Mockup -> Fahrdynamik
				#
				Mockup.Ignition <-> VDyn.Ignition,
				Mockup.Starter <-> VDyn.Starter,
				Mockup.HandBrake <-> VDyn.HandBrake,
				Mockup.Gear <-> VDyn.Gear,
				Mockup.SteeringWheel <-> VDyn.SteeringWheel,
				#Mockup.AcceleratorPedal<-> VDyn.AcceleratorPedal,
				Mockup.BrakePedal <-> VDyn.BrakePedal,

				#
				# Verbindung Fahrdynamik -> Mockup/Kombidisplay
				#
				VDyn.v <-> Display.v,
				VDyn.RPM <-> Display.RPM
			};
			
			DPURubyScript ~Tempomat
			{
				Computer = {VDYN};
				Index = 10;
				Script = "scripts/RPUTempomat.rb";
			};
			
			Connections =
			{
				VDyn.v_kmh -> ~Tempomat.Vkmh
				,Mockup.AcceleratorPedal -> ~Tempomat.AcceleratorPedal
				,~Tempomat.AcceleratorPedalnew -> VDyn.AcceleratorPedal
			};