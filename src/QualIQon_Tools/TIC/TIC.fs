namespace QualIQon.Tools.TIC

open System
open ProteomIQon
open Plotly.NET
open MzIO.IO.MzML
open MzIO.Processing
open MzIO.Model
open QualIQon.Plots
open QualIQon.IO
   
module createTICPlot = 
    let TIC = QualIQon.Plots.TICPlot.create