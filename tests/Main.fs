module QualIQon.Tests

open Expecto
open QualIQon.IO
open QualIQon.Plots

[<EntryPoint>]
let main argv =
    runTestsWithArgs defaultConfig argv corrHeavyTests
    
