// See https://aka.ms/new-console-template for more information

using Spectre.Console.Cli;
using Weavly.Cli.Commands.Core;
using Weavly.Cli.Commands.Module;

var app = new CommandApp();

app.Configure(c =>
{
    c.Settings.ApplicationName = "wv";

    c.AddCommand<InitCommand>("init");
    c.AddCommand<UpdateCommand>("update");
    c.AddCommand<InfoCommand>("info");
    c.AddCommand<FormatCommand>("format");
    c.AddBranch(
        "module",
        m =>
        {
            m.SetDescription("Manages modules");
            m.AddCommand<AddCommand>("add");
            m.AddCommand<NewCommand>("new");
        }
    );
});

await app.RunAsync(args);
