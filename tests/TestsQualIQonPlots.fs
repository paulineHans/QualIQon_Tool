module TestsPlots

open Expecto
open QualIQon.Plots
open QualIQon.IO

[<Tests>]

let mkTempDir () =
    let d = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.Guid.NewGuid().ToString("N"))
    System.IO.Directory.CreateDirectory(d) |> ignore
    d


let tests = 
    testList "QualIQon.Plots" [
        testCase "quantHeavy returns a chart (smoke test)" <| fun _ ->
            let dir =mkTempDir()
            let chart = QualIQon.Plots.CorrHeavyPlot.quantHeavy dir "ProteomIQon"
            Expect.isNotNull (box chart) "Should return a chart object"

        testCase "quantLight returns a chart (smoke test)" <| fun _ ->
            let dir = mkTempDir()
            let chart = QualIQon.Plots.CorrLightPlot.quantLight dir "ProteomIQon"
            Expect.isNotNull (box chart) "Should return a chart object"
        
        testCase "quantRatio returns a chart (smoke test)" <| fun _ ->
            let dir = mkTempDir()
            let chart = QualIQon.Plots.CorrRatioPlot.quantRatio dir "ProteomIQon"
            Expect.isNotNull (box chart) "Should return a chart object"

        testCase "CorrelationLightHeavy throws on empty directory" <| fun _ ->
            let dir = mkTempDir()
            Expect.throws
                (fun () -> QualIQon.Plots.CorrelationLightHeavyPlot.CorrLHP dir |> ignore)
                "Empty directory should throw"
        
        testCase "Misscleavages throws on empty directory" <| fun _ ->
            let dir = mkTempDir()
            Expect.throws
                (fun () -> QualIQon.Plots.MisscleavagesPlot.MC dir |> ignore)
                "Empty directory should throw"
        
        testCase "ScoreRefinement returns a chart (smoke test)" <| fun _ ->
            let dir = mkTempDir()
            let chart = QualIQon.Plots.ScoreRefinementPlot.SR dir
            Expect.isNotNull (box chart) "Should return a chart object"
        
        testCase "Protein Identification returns a chart (smoke test)" <| fun _ ->
            let dir = mkTempDir()
            let chart = QualIQon.Plots.ProteinIdentificationPlot.PI dir "ProteomIQon"
            Expect.isNotNull (box chart) "Should return a chart object"
        
        testCase "XIC throws on empty directory" <| fun _ ->
            let dir = mkTempDir()
            Expect.throws
                (fun () -> QualIQon.Plots.XICPlot.createXIC dir |> ignore)
                "Empty directory should throw"

        
        testCase "TIC returns a chart (smoke test)" <| fun _ ->
            let dir = mkTempDir()
            let chart = QualIQon.Plots.TICPlot.create dir
            Expect.isNotNull (box chart) "Should return a chart object"
        
        testCase "MS1Map throws on empty directory" <| fun _ ->
            let dir = mkTempDir()
            Expect.throws
                (fun () -> QualIQon.Plots.MS1MapPlot.createMS1Map dir |> ignore)
                "Empty directory should throw"

    ]