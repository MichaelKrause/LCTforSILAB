# RPUTempomat
# Tempomat für LCT
# Erstellt: 31.05.2010 (SILABDPUWizard)
# Autor: C. Rommerskirchen

# Die Klasse RPUTempomat ist die Hauptklasse der RPU. 
# Ihre Methoden werden von SILAB während der Simulation aufgerufen. 
class RPUTempomat < RPU

    # In der Initialize-Methode angegebene Befehle werden 
    # eimal beim Laden der Versuchskonfiguration ausgeführt. 
	# In dieser Methode	sollten die Eingangs- und Ausgangsvariablen 
	# der RPU durch Aufrufe von  var_in  bzw.  var_out  erzeugt werden. 
	def initialize
		# Eingänge der RPU
		var_in :@Vkmh, :@AcceleratorPedal
		# Ausgänge der RPU
		var_out :@AcceleratorPedalnew
	end

	# Wird aufgerufen, sobald der Benutzer 'Launch' wählt.
	# Aufgabe: Vorbereitungsschritte, die bei jedem Start einer Simulation durchgeführt werden müssen.
	# Rückgabe: true  -> Vorbereitungen waren erfolgreich.
	# 			 false -> Bei den Vorbereitungen ist ein Fehler aufgetreten.
	def start
		return true
	end
	
	# Wird periodisch aufgerufen, während die Simulation läuft.
	# Aufgabe: Funktionalität der RPU während der Simulation. Die Vorgänge in dieser Methode sollten
	#          nicht länger als ca. 1 ms dauern.
	# Parameter: time       -> Zeit seit dem Start der Simulation (in Millisekunden)
	#            time_error -> Aktuelle Rechenzeitverzögerung (in Millisekunden)
	def trigger(time, time_error)
# 		if (@Vkmh>60)
# 			@AcceleratorPedalnew =0.0
# 		else
# 			@AcceleratorPedalnew = @AcceleratorPedal
# 		end
		@AcceleratorPedalnew=(60-@Vkmh)/2*@AcceleratorPedal
		
	end
	
end
