using System;
using System.Collections.Generic;
using System.Text;

namespace PoissonSoft.CommonUtils.Environments
{
    /// <summary>
    /// Wrapper for run and manage docker containers
    /// </summary>
    public sealed class DockerContainerWrap : IDisposable
    {
        /// <summary>
        /// Settings of the container
        /// </summary>
        public DockerContainerSettings Settings { get; private set; }
        /// <summary>
        /// State of the container
        /// </summary>
        public DockerContainerState ContainerState { get; private set; }
        /// <summary>
        /// Container id
        /// </summary>
        public string ContainerId { get; private set; }

        /// <summary>
        /// Сreates a container with the given settings
        /// </summary>
        /// <param name="settings">container settings</param>
        public DockerContainerWrap(DockerContainerSettings settings)
        {
            Settings = settings;
            if (settings.PublicPort.HasValue == false)
            {
                settings.PublicPort = (new Random()).Next(15000, 30000);
            }
            if (string.IsNullOrWhiteSpace(settings.ContainerName) == false)
                RemoveContainer(true);
            ContainerState = DockerContainerState.NotExits;
        }

        /// <summary>
        /// Remove the container
        /// </summary>
        /// <param name="force">Ignore errors</param>
        public void RemoveContainer(bool force = true)
        {
            var idOrName = ContainerId ?? Settings.ContainerName;
            if (string.IsNullOrWhiteSpace(idOrName) == true)
            {
                throw new InvalidOperationException("The attempt to delete a container that has no id and name defined");
            }
            var res = TerminalHelper.RunCommand("docker", $"rm -f {idOrName}");
            if (res.HasError == true && force == false)
            {
                throw new InvalidOperationException("Error while deleting container: " + res.ErrorOutput);
            }
            ContainerState = DockerContainerState.NotExits;
        }

        /// <summary>
        /// Get arguments for run the container
        /// </summary>
        /// <returns>Arguments for run the container</returns>
        public string GetDockerRunArguments()
        {
            var sb = new StringBuilder();
            sb.Append("run -d");
            if (string.IsNullOrWhiteSpace(Settings.ContainerName) == false)
            {
                sb.Append(" --name " + Settings.ContainerName);
            }
            if (Settings.InternalPort.HasValue == true)
            {
                sb.Append($" -p {Settings.PublicPort.Value}:{Settings.InternalPort.Value}");
            }
            if (Settings.Variables != null)
            {
                foreach (var ev in Settings.Variables)
                {
                    sb.Append(" -e " + ev.Item1 + "=" + ev.Item2);
                }
            }
            sb.Append(" " + Settings.Image);
            if (Settings.EntryPointParameters != null)
            {
                foreach (var p in Settings.EntryPointParameters)
                {
                    sb.Append(" " + p);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Run the container
        /// </summary>
        public void StartContainer()
        {
            if (ContainerState == DockerContainerState.Running)
            {
                throw new InvalidOperationException($"Контейнер {Settings.ContainerName} уже запущен.");
            }
            else if (ContainerState == DockerContainerState.NotExits)
            {
                var arguments = GetDockerRunArguments();
                var res = TerminalHelper.RunCommand("docker", arguments);
                if (res.HasError == true)
                {
                    throw new InvalidOperationException("Не удалось запустить контейнер. Ошибка: " + res.ErrorOutput);
                }
                ContainerId = res.StandardOutput;
                ContainerState = DockerContainerState.Running;
            }
            else if (ContainerState == DockerContainerState.Stopped)
            {
                throw new NotImplementedException("Не реализован запуск остановленного контейнера");
            }
            else
            {
                throw new InvalidOperationException("Что-то пошло не так...");
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            RemoveContainer(true);
        }
    }

    /// <summary>
    /// Settings to run the container
    /// </summary>
    public class DockerContainerSettings
    {
        /// <summary>
        /// Image name (include the tag, if it is needed)
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// Container name 
        /// </summary>
        public string ContainerName { get; set; }
        /// <summary>
        /// Port number inside the container
        /// </summary>
        public int? InternalPort { get; set; }
        /// <summary>
        /// Port on the host
        /// </summary>
        public int? PublicPort { get; set; }
        /// <summary>
        /// Environment variables for pass to the container
        /// </summary>
        public (string, string)[] Variables { get; set; }
        /// <summary>
        /// Custom parameters
        /// </summary>
        public string[] EntryPointParameters { get; set; }
    }

    /// <summary>
    /// State of the container
    /// </summary>
    public enum DockerContainerState
    {
        /// <summary>
        /// Container does not exist
        /// </summary>
        NotExits = 1,
        /// <summary>
        /// Container runs
        /// </summary>
        Running = 2,
        /// <summary>
        /// Container stopped
        /// </summary>
        Stopped = 3
    }
}
