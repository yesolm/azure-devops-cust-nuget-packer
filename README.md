# azure-devops-cust-nuget-packer
I been through some Azure build pipeline tool, isntalled assembly & versioning tools , other marketplace tools with out success so i decided to build my own console app that does exactly what i want. It's configurable but not perfect, will update more when i have the time.

### To start 
. rename appsettings.example.json to appsettings.json
. Add ```"EnableMultiFramworkPackagiing":  false``` in the top level of ```"NugetPackSettings"```
### Note
. Currently working in Azure, you can also run it locally 
. Not thoroghly tasted, but easily configurable
. If you have any questions, let me know. 

## My Build Pipeline .yml file

```
trigger:
  - master
  - develop
  - azure-pipelines

pool:
  vmImage: 'windows-latest'

variables:
  buildConfiguration: 'Release'

steps:
- task: DotNetCoreCLI@2
  displayName: 'NuGet Restore'
  inputs:
    command: restore
    projects:  '$(Parameters.projects)' 

- task: DotNetCoreCLI@2
  inputs:
    command: 'build'
    projects: '$(Parameters.projects)'
    arguments: '--configuration $(BuildConfiguration)'
- task: DotNetCoreCLI@2
  inputs:
    command: 'publish'
    publishWebProjects: false
    projects: '$(Parameters.projects)'
    arguments: '--output $(Build.ArtifactStagingDirectory)/bin'
    zipAfterPublish: false
    modifyOutputPath: false
- task: DownloadGitHubRelease@0
  inputs:
    connection: 'GitHub'
    userRepository: 'yesolm/azuredevops.peronal'
    defaultVersionType: 'latest'
    downloadPath: '$(System.ArtifactsDirectory)'

- task: CmdLine@2
  inputs:
    script: 'azdevopscustnuget.exe -b $(Build.ArtifactStagingDirectory) -s  $(Build.SourcesDirectory) -n 
    $(Build.ArtifactStagingDirectory)/Nupkg'
    workingDirectory: '$(System.ArtifactsDirectory)'
```
