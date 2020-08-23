﻿using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Features.Metadata;
using GreyOTron.Library.Commands;
using GreyOTron.Library.Helpers;
using GreyOTron.Library.Interfaces;

namespace GreyOTron
{
    public class CommandProcessor
    {
        private readonly string prefix;
        private readonly ILifetimeScope container;

        public CommandProcessor(string prefix, ILifetimeScope container)
        {
            this.prefix = prefix;
            this.container = container;
        }

        public ICommand Parse(string message)
        {
            message = message.Trim();
            if (!message.StartsWith(prefix))
            {
                return new NullCommand();
            }

            if (EnvironmentHelper.Is(Environments.Maintenance))
            {
                return new MaintenanceCommand();
            }
            
            var commandName = message[prefix.Length..];
            var i = commandName.IndexOf(' ');
            if (i >= 0)
            {
                message = commandName[(i+1)..];
                commandName = commandName.Substring(0, i).ToLowerInvariant();
            }
            else
            {
                commandName = commandName.Trim().ToLowerInvariant();
                message = string.Empty;
            }
            var command = container.Resolve<IEnumerable<Meta<ICommand>>>()
                .FirstOrDefault(a => a.Metadata.ContainsKey("CommandName") && a.Metadata["CommandName"].Equals(commandName))?.Value;
            if (command != null)
            {
                command.Arguments = message;
            }
            else
            {
                command = new NotFoundCommand { Arguments = commandName };
            }
            return command;
        }
    }
}
