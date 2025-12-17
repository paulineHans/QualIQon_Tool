namespace QualIQon.Tools.CorrRatio


open Argu
open CLI_Parsing
open System.Reflection
open createCorrRatioPlot

module console_Corr_Ratio = 
    [<EntryPoint>]
    let main argv =
            let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
            let parser = ArgumentParser.Create<CLIArguments>(programName =  (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name),errorHandler=errorHandler)     
            let results = parser.Parse argv
            let i = results.GetResult DirectoryPath 
            let o = results.GetResult Pipeline
            let p = results.GetResult LabeledData        
            let execute_Corr_Ratio = 
                let labeling = p
                let checkExecution = 
                    if labeling = "15N" then 
                        Some (HeatmapCorrelationRatio  i o )
                    else  
                        None
                checkExecution
                0
            execute_Corr_Ratio