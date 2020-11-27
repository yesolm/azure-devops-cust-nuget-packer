using System;
using System.Collections.Generic;
using System.Text;

namespace Yesolm.DevOps.Models
{
    public enum ExitCode
    {
        _Defualt = -1,
        OK = 0,
        UnhandledException = 1,
        NugetDirectoryAccessException = 2,
        NoPojectFilesFound = 3,
    }

    public enum SolutionConfigType
    {
        Debug,
        Release
    }
    public enum Rule
    {
        NotEmpty,
        NotEmptyValidateChildren,
        MustBeADefinedEnum
    }

    public enum CommandOption
    {
        BUILD_DIR,
        SOURCE_DIR,
        NUGET_OUT_DIR
    }

    public enum BuildAction
    {
        None = 0 ,
        Content = 1 ,
        Resource = 3 ,
        Embeded_Resource = 4,
        Compile = 5
    }
}
