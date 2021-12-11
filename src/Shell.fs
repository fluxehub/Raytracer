namespace Raytracer

/// This is the main module of your application
/// here you handle all of your child pages as well as their
/// messages and their updates, useful to update multiple parts
/// of your application, Please refer to the `view` function
/// to see how to handle different kinds of "*child*" controls
module Shell =
    open Elmish
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.FuncUI
    open Avalonia.FuncUI.Components.Hosts
    open Avalonia.FuncUI.Elmish

    type State = { renderState: RenderControl.State }

    type Msg = RenderControlMsg of RenderControl.Msg

    let init =
        let renderControlState = RenderControl.init 800 600
        { renderState = renderControlState }, Cmd.none

    let update (msg: Msg) (state: State) : State * Cmd<_> =
        match msg with
        | RenderControlMsg renderMsg ->
            let renderControlMsg, cmd = RenderControl.update renderMsg state.renderState
            { state with renderState = renderControlMsg }, Cmd.map RenderControlMsg cmd

    let view (state: State) dispatch =
        DockPanel.create [ DockPanel.children [ RenderControl.view state.renderState (RenderControlMsg >> dispatch) ] ]

    type MainWindow() as this =
        inherit HostWindow()

        do
            base.Title <- "Raytracer"
            base.Width <- 1000.0
            base.Height <- 800.0
            base.MinWidth <- 800.0
            base.MinHeight <- 600.0

            // this.VisualRoot.VisualRoot.Renderer.DrawFps <- true
            // this.VisualRoot.VisualRoot.Renderer.DrawDirtyRects <- true

            Elmish.Program.mkProgram (fun () -> init) update view
            |> Program.withHost this
            |> Program.run
