namespace QualIQon.Tools.CorrLight

open System
open System.IO
open Deedle
open FSharp.Stats
open Plotly.NET
open FSharpAux
open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects
open QualIQon.IO
open QualIQon.Plots

module createCorrLightPlot = 
    let HeatmapCorrelationLight = QualIQon.Plots.CorrLightPlot.quantLight    

        
    