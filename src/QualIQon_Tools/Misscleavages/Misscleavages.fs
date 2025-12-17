namespace QualIQon.Tools.Misscleavages

open ProteomIQon
open ProteomIQon.Dto
open System 
open System.IO 
open Plotly.NET 
open Plotly.NET.TraceObjects
open Plotly.NET.LayoutObjects
open Deedle
open QualIQon.IO
open QualIQon.Plots 

module createMisscleavagePlot = 
    let MCPlot = QualIQon.Plots.MisscleavagesPlot.MC
