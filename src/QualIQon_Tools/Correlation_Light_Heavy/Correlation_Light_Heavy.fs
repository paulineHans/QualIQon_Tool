namespace QualIQon.Tool.CorrLHP

open ProteomIQon.Dto
open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects
open Plotly.NET 
open FSharpAux.IO.SchemaReader.Attribute
open System
open QualIQon.IO
open QualIQon.Plots

module createCorrelationLightHeavyPlot = 
    let CorrelationLightHeavy = QualIQon.Plots.CorrelationLightHeavyPlot.CorrLHP