namespace PocoGen

open Fabulous.XamarinForms
open Xamarin.Forms

module Components =
    let formLabel text =
        View.Label(text = text)

    let formEntry text textChanged =
        View.Entry(text = text,
                textChanged = (fun e -> e.NewTextValue |> textChanged),
                height = 100.)

    let formEditor text textChanged =
        View.Editor(text = text,
                    textChanged = (fun e -> e.NewTextValue |> textChanged),
                    height = 100.)
    let formButton text cmd isEnabled =
        View.Button(isEnabled = isEnabled, text = sprintf text, command = cmd)

    let formStack children =
        View.StackLayout(
            orientation = StackOrientation.Vertical,
            verticalOptions = LayoutOptions.FillAndExpand,
            horizontalOptions = LayoutOptions.FillAndExpand,
            padding = Thickness(20.0),
            margin = Thickness(20.0),
            children = children)

