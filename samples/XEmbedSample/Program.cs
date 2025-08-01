﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using ControlCatalog;
using ControlCatalog.Models;
using Gdk;
using Gtk;
using Window = Gtk.Window;

namespace XEmbedSample;

class Program
{
    static void Main(string[] args)
    {
        HarfbuzzWorkaround.Apply();
        AppBuilder.Configure<App>()
            .UseSkia()
            .With(new X11PlatformOptions()
            {
                UseGLibMainLoop = true,
                ExternalGLibMainLoopExceptionLogger = e => Console.WriteLine(e.ToString())
            })
            .UseX11()
            .SetupWithoutStarting();
        App.SetCatalogThemes(CatalogTheme.Fluent);
        Gdk.Global.AllowedBackends = "x11";
        Gtk.Application.Init("myapp", ref args);





        var w = new Gtk.Window("XEmbed Test Window");
        var socket = new AvaloniaXEmbedGtkSocket(GetBackgroundColor(w))
        {
            Content = new ScrollViewer()
            {
                Content = new ControlCatalog.Pages.TextBoxPage(),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            }
        };
        var vbox = new Gtk.Box(Gtk.Orientation.Vertical, 5);
        var label = new Gtk.Label("Those are GTK controls");
        vbox.Add(label);
        vbox.Add(new Gtk.Entry());
        vbox.Add(new Gtk.Button(new Gtk.Label("Do nothing")));
        vbox.PackEnd(socket, true, true, 0);
        socket.HeightRequest = 400;
        socket.WidthRequest = 400;
        w.Add(vbox);
        socket.Realize();

        
        w.AddSignalHandler("destroy", new EventHandler((_, __) =>
        {
            Gtk.Application.Quit();
            socket.Destroy();
        }));
        w.ShowAll();
        Gtk.Application.Run();
        
    }

    private static RGBA GetBackgroundColor(Window window)
    {
#pragma warning disable CS0612 // Type or member is obsolete; OK for a sample.
        return window.StyleContext.GetBackgroundColor(StateFlags.Normal);
#pragma warning restore CS0612 // Type or member is obsolete
    }
}
