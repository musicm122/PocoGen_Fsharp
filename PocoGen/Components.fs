﻿namespace PocoGen.Components

open Fabulous.XamarinForms
open Xamarin.Forms

module Components =
    let formLabel text = View.Label(text = text)

    let formOutputLabel text =
        View.Label
            (text = text,
             backgroundColor = Color.Black,
             textColor = Color.White,
             verticalOptions = LayoutOptions.FillAndExpand,
             horizontalOptions = LayoutOptions.FillAndExpand)

    let formOutput text =
        View.ScrollView
            (verticalOptions = LayoutOptions.FillAndExpand, 
             horizontalOptions = LayoutOptions.FillAndExpand, 
             content =
                View.StackLayout
                    (verticalOptions = LayoutOptions.FillAndExpand,
                     horizontalOptions = LayoutOptions.FillAndExpand,
                     children = [ formOutputLabel text ]))

    let formEntry text textChanged =
        View.Entry(text = text, textChanged = (fun e -> e.NewTextValue |> textChanged), height = 30.)

    let formEditor text textChanged =
        View.Editor(text = text, textChanged = (fun e -> e.NewTextValue |> textChanged), height = 100.)

    let formMultiLineEditor text textChanged =
        View.Editor(text = text, textChanged = (fun e -> e.NewTextValue |> textChanged), height = 100.)

    let formButton text cmd isEnabled =
        View.Button
            (verticalOptions = LayoutOptions.Fill,
             horizontalOptions = LayoutOptions.FillAndExpand,
             isEnabled = isEnabled,
             text = sprintf text,
             command = cmd)

    let List col =
        View.ListView
            (items =
                [ for item in col do
                    yield View.TextCell(item) ])

    let verticalStack children =
        View.StackLayout
            (orientation = StackOrientation.Vertical,
             verticalOptions = LayoutOptions.FillAndExpand,
             horizontalOptions = LayoutOptions.FillAndExpand,
             padding = Thickness(20.0),
             margin = Thickness(20.0),
             children = children)

    let horizontalStack children =
        View.StackLayout
            (orientation = StackOrientation.Horizontal,
             verticalOptions = LayoutOptions.StartAndExpand,
             horizontalOptions = LayoutOptions.StartAndExpand,
             padding = Thickness(20.0),
             margin = Thickness(20.0),
             children = children)
