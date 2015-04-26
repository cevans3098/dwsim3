Imports System.IO
Imports Nini.Config
Imports System.Globalization
Imports System.Reflection
Imports System.Linq
Imports Cudafy.Translator
Imports Cudafy
Imports Cudafy.Host

'    Shared Functions
'    Copyright 2008-2014 Daniel Wagner O. de Medeiros
'
'    This file is part of DWSIM.
'
'    DWSIM is free software: you can redistribute it and/or modify
'    it under the terms of the GNU General Public License as published by
'    the Free Software Foundation, either version 3 of the License, or
'    (at your option) any later version.
'
'    DWSIM is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    GNU General Public License for more details.
'
'    You should have received a copy of the GNU General Public License
'    along with DWSIM.  If not, see <http://www.gnu.org/licenses/>.

Namespace DWSIM

    <System.Serializable()> Public Class App

        Public Shared Sub WriteToConsole(text As String, minlevel As Integer)

            If My.Settings.RedirectOutput Then
                My.Application.OpenForms(0).UIThread(Sub()
                                                         If My.Settings.DebugLevel >= minlevel Then
                                                             If Not My.Application.ActiveSimulation Is Nothing Then
                                                                 If Not My.Application.ActiveSimulation.FormOutput Is Nothing Then
                                                                     My.Application.ActiveSimulation.FormOutput.TextBox1.AppendText(text & vbCrLf)
                                                                 End If
                                                             End If
                                                         End If
                                                     End Sub)
            Else
                If My.Settings.DebugLevel >= minlevel Then Console.WriteLine(text)
            End If

        End Sub

        Public Shared Sub HelpRequested(topic As String)
            Dim pathsep = System.IO.Path.DirectorySeparatorChar
            Dim filename As String = My.Application.Info.DirectoryPath & pathsep & "help" & pathsep & topic
            System.Diagnostics.Process.Start(filename)
        End Sub

        Public Shared Function GetLocalTipString(ByVal id As String) As String

            If My.MyApplication._HelpManager Is Nothing Then

                'loads the current language
                My.MyApplication._CultureInfo = New Globalization.CultureInfo(My.Settings.CultureInfo)
                My.Application.ChangeUICulture(My.Settings.CultureInfo)

                'loads the resource manager
                My.MyApplication._HelpManager = New System.Resources.ResourceManager("DWSIM.Tips", System.Reflection.Assembly.GetExecutingAssembly())

            End If

            If id <> "" Then
                Dim retstr As String
                retstr = My.MyApplication._HelpManager.GetString(id, My.MyApplication._CultureInfo)
                If retstr Is Nothing Then Return id Else Return retstr
            Else
                Return ""
            End If
        End Function


        Public Shared Function GetLocalString(ByVal id As String) As String

            If My.MyApplication._ResourceManager Is Nothing Then

                'loads the current language
                My.MyApplication._CultureInfo = New Globalization.CultureInfo(My.Settings.CultureInfo)
                My.Application.ChangeUICulture(My.Settings.CultureInfo)

                'loads the resource manager
                My.MyApplication._ResourceManager = New System.Resources.ResourceManager("DWSIM.DWSIM", System.Reflection.Assembly.GetExecutingAssembly())

            End If

            If id <> "" Then
                Dim retstr As String
                retstr = My.MyApplication._ResourceManager.GetString(id, My.MyApplication._CultureInfo)
                If retstr Is Nothing Then Return id Else Return retstr
            Else
                Return ""
            End If
        End Function

        Public Shared Function GetPropertyName(ByVal PropID As String, Optional ByRef fp As FormMain = Nothing) As String

            If My.MyApplication._ResourceManager Is Nothing Then

                'loads the current language
                My.MyApplication._CultureInfo = New Globalization.CultureInfo(My.Settings.CultureInfo)
                My.Application.ChangeUICulture(My.Settings.CultureInfo)

                'loads the resource manager
                My.MyApplication._ResourceManager = New System.Resources.ResourceManager("DWSIM.DWSIM", System.Reflection.Assembly.GetExecutingAssembly())

            End If

            'loads the property name manager
            If My.MyApplication._PropertyNameManager Is Nothing Then

                My.MyApplication._PropertyNameManager = New System.Resources.ResourceManager("DWSIM.Properties", System.Reflection.Assembly.GetExecutingAssembly())

            End If

            Dim retstr As String
            If Not PropID Is Nothing Then
                Dim prop As String = PropID.Split(",")(0)
                Dim sname As String = ""
                If PropID.Split(",").Length = 2 Then
                    sname = PropID.Split(",")(1)
                    retstr = My.MyApplication._PropertyNameManager.GetString(prop, My.MyApplication._CultureInfo) + " - " + DWSIM.App.GetComponentName(sname, fp)
                    If retstr Is Nothing Then Return PropID Else Return retstr
                Else
                    retstr = My.MyApplication._PropertyNameManager.GetString(prop, My.MyApplication._CultureInfo)
                    If retstr Is Nothing Then Return PropID Else Return retstr
                End If
            Else
                retstr = ""
            End If

            Return Nothing

        End Function

        Public Shared Function GetComponentName(ByVal UniqueName As String, Optional ByRef fp As FormMain = Nothing, Optional ByVal COmode As Boolean = False) As String
            If COmode Then
                Dim str As String = GetLocalString("_" + UniqueName)
                If str(0) = "_" Then
                    Return UniqueName
                Else
                    Return str
                End If
            Else
                If Not UniqueName = "" Then
                    If fp Is Nothing Then fp = My.Forms.FormMain
                    If fp.AvailableComponents.ContainsKey(UniqueName) Then
                        Dim str As String = GetLocalString("_" + UniqueName)
                        If UniqueName Is Nothing Then
                            Return fp.AvailableComponents.Item(UniqueName).Name
                        Else
                            If str(0) = "_" Then
                                Return UniqueName
                            Else
                                Return str
                            End If
                        End If
                    Else
                        Dim frmc As FormFlowsheet = My.Application.ActiveSimulation
                        If Not frmc Is Nothing Then
                            If frmc.Options.SelectedComponents.ContainsKey(UniqueName) Then
                                Return frmc.Options.SelectedComponents(UniqueName).Name
                            ElseIf frmc.Options.NotSelectedComponents.ContainsKey(UniqueName) Then
                                Return frmc.Options.NotSelectedComponents(UniqueName).Name
                            Else
                                Return UniqueName
                            End If
                        Else
                            Return UniqueName
                        End If
                    End If
                Else
                    Return UniqueName
                End If
                Return UniqueName
            End If
        End Function

        Public Shared Function GetComponentType(ByRef comp As DWSIM.ClassesBasicasTermodinamica.ConstantProperties) As String
            If comp.IsHYPO Then
                Return GetLocalString("CompHypo")
            ElseIf comp.IsPF Then
                Return GetLocalString("CompPseudo")
            Else
                Return GetLocalString("CompNormal")
            End If
        End Function

        Public Shared Function IsRunningOnMono() As Boolean
            Return Not Type.GetType("Mono.Runtime") Is Nothing
        End Function

        Shared Sub LoadSettings(Optional ByVal configfile As String = "")

            If configfile = "" Then configfile = My.Application.Info.DirectoryPath + Path.DirectorySeparatorChar + "dwsim.ini"
            If Not File.Exists(configfile) Then File.Copy(My.Application.Info.DirectoryPath + Path.DirectorySeparatorChar + "default.ini", configfile)

            Dim source As New IniConfigSource(configfile)
            Dim col() As String

            My.Settings.MostRecentFiles = New Collections.Specialized.StringCollection()

            With source
                col = .Configs("RecentFiles").GetValues()
                For Each Str As String In col
                    My.Settings.MostRecentFiles.Add(Str)
                Next
            End With

            My.Settings.ScriptPaths = New Collections.Specialized.StringCollection()

            With source
                col = .Configs("ScriptPaths").GetValues()
                For Each Str As String In col
                    My.Settings.ScriptPaths.Add(Str)
                Next
            End With

            My.Settings.UserDatabases = New Collections.Specialized.StringCollection()

            With source
                col = .Configs("UserDatabases").GetValues()
                For Each Str As String In col
                    My.Settings.UserDatabases.Add(Str)
                Next
            End With

            My.Settings.UserInteractionsDatabases = New Collections.Specialized.StringCollection()

            With source
                col = .Configs("UserInteractionsDatabases").GetValues()
                For Each Str As String In col
                    My.Settings.UserInteractionsDatabases.Add(Str)
                Next
            End With

            My.Settings.BackupFiles = New Collections.Specialized.StringCollection()

            With source
                col = .Configs("BackupFiles").GetValues()
                For Each Str As String In col
                    My.Settings.BackupFiles.Add(Str)
                Next
            End With

            My.Settings.BackupActivated = source.Configs("Backup").GetBoolean("BackupActivated", True)
            My.Settings.BackupFolder = source.Configs("Backup").Get("BackupFolder", My.Computer.FileSystem.SpecialDirectories.Temp + Path.DirectorySeparatorChar + "DWSIM")
            My.Settings.BackupInterval = source.Configs("Backup").GetInt("BackupInterval", 5)

            My.Settings.CultureInfo = source.Configs("Localization").Get("CultureInfo", "en-US")
         
            My.Settings.ChemSepDatabasePath = source.Configs("Databases").Get("ChemSepDBPath", "")
            My.Settings.ReplaceComps = source.Configs("Databases").GetBoolean("ReplaceComps", True)

            My.Settings.UserUnits = source.Configs("Misc").Get("UserUnits", "")
            My.Settings.ShowTips = source.Configs("Misc").GetBoolean("ShowTips", True)
            My.Settings.RedirectOutput = source.Configs("Misc").GetBoolean("RedirectConsoleOutput", False)

            My.Settings.EnableParallelProcessing = source.Configs("Misc").GetBoolean("EnableParallelProcessing", False)
            My.Settings.MaxDegreeOfParallelism = source.Configs("Misc").GetInt("MaxDegreeOfParallelism", -1)
            My.Settings.EnableGPUProcessing = source.Configs("Misc").GetBoolean("EnableGPUProcessing", False)
            My.Settings.SelectedGPU = source.Configs("Misc").Get("SelectedGPU", "")
            My.Settings.CudafyTarget = source.Configs("Misc").GetInt("CudafyTarget", 1)
            My.Settings.CudafyDeviceID = source.Configs("Misc").GetInt("CudafyDeviceID", 0)

            My.Settings.DebugLevel = source.Configs("Misc").GetInt("DebugLevel", 0)
            My.Settings.SolverMode = source.Configs("Misc").GetInt("SolverMode", 0)
            My.Settings.ServiceBusConnectionString = source.Configs("Misc").Get("ServiceBusConnectionString", "")
            My.Settings.ServerIPAddress = source.Configs("Misc").Get("ServerIPAddress", "")
            My.Settings.ServerPort = source.Configs("Misc").Get("ServerPort", "")
            My.Settings.SolverTimeoutSeconds = source.Configs("Misc").GetInt("SolverTimeoutSeconds", 300)


        End Sub

        Shared Sub SaveSettings(Optional ByVal configfile As String = "")

            If configfile = "" Then
                configfile = My.Application.Info.DirectoryPath + Path.DirectorySeparatorChar + "dwsim.ini"
                File.Copy(My.Application.Info.DirectoryPath + Path.DirectorySeparatorChar + "default.ini", configfile, True)
            Else
                File.Copy(My.Application.Info.DirectoryPath + Path.DirectorySeparatorChar + "excelcompat.ini", configfile, True)
            End If


            Dim source As New IniConfigSource(configfile)

            For Each Str As String In My.Settings.MostRecentFiles
                source.Configs("RecentFiles").Set(My.Settings.MostRecentFiles.IndexOf(Str), Str)
            Next

            For Each Str As String In My.Settings.ScriptPaths
                source.Configs("ScriptPaths").Set(My.Settings.ScriptPaths.IndexOf(Str), Str)
            Next

            For Each Str As String In My.Settings.UserDatabases
                source.Configs("UserDatabases").Set(My.Settings.UserDatabases.IndexOf(Str), Str)
            Next

            For Each Str As String In My.Settings.UserInteractionsDatabases
                source.Configs("UserInteractionsDatabases").Set(My.Settings.UserInteractionsDatabases.IndexOf(Str), Str)
            Next

            For Each Str As String In My.Settings.BackupFiles
                source.Configs("BackupFiles").Set(My.Settings.BackupFiles.IndexOf(Str), Str)
            Next

            source.Configs("Backup").Set("BackupActivated", My.Settings.BackupActivated)
            source.Configs("Backup").Set("BackupFolder", My.Settings.BackupFolder)
            source.Configs("Backup").Set("BackupInterval", My.Settings.BackupInterval)

            source.Configs("Localization").Set("CultureInfo", My.Settings.CultureInfo)
          
            source.Configs("Databases").Set("ChemSepDBPath", My.Settings.ChemSepDatabasePath)
            source.Configs("Databases").Set("ReplaceComps", My.Settings.ReplaceComps)

            source.Configs("Misc").Set("UserUnits", My.Settings.UserUnits)
            source.Configs("Misc").Set("ShowTips", My.Settings.ShowTips)
            source.Configs("Misc").Set("RedirectConsoleOutput", My.Settings.RedirectOutput)

            source.Configs("Misc").Set("EnableParallelProcessing", My.Settings.EnableParallelProcessing)
            source.Configs("Misc").Set("MaxDegreeOfParallelism", My.Settings.MaxDegreeOfParallelism)
            source.Configs("Misc").Set("EnableGPUProcessing", My.Settings.EnableGPUProcessing)
            source.Configs("Misc").Set("SelectedGPU", My.Settings.SelectedGPU)
            source.Configs("Misc").Set("CudafyTarget", My.Settings.CudafyTarget)
            source.Configs("Misc").Set("CudafyDeviceID", My.Settings.CudafyDeviceID)

            source.Configs("Misc").Set("DebugLevel", My.Settings.DebugLevel)
            source.Configs("Misc").Set("SolverMode", My.Settings.SolverMode)
            source.Configs("Misc").Set("ServiceBusConnectionString", My.Settings.ServiceBusConnectionString)
            source.Configs("Misc").Set("ServerIPAddress", My.Settings.ServerIPAddress)
            source.Configs("Misc").Set("ServerPort", My.Settings.ServerPort)
            source.Configs("Misc").Set("SolverTimeoutSeconds", My.Settings.SolverTimeoutSeconds)

            source.Save(configfile)

        End Sub

        Shared Sub InitComputeDevice()

            If My.MyApplication.gpu Is Nothing Then

                'set target language

                Select Case My.Settings.CudafyTarget
                    Case 0, 1
                        CudafyTranslator.Language = eLanguage.Cuda
                    Case 2
                        CudafyTranslator.Language = eLanguage.OpenCL
                End Select

                'get the gpu instance

                Dim gputype As eGPUType = My.Settings.CudafyTarget

                My.MyApplication.gpu = CudafyHost.GetDevice(gputype, My.Settings.CudafyDeviceID)

                'cudafy all classes that contain a gpu function

                If My.MyApplication.gpumod Is Nothing Then
                    Select Case My.Settings.CudafyTarget
                        Case 0, 1
                            My.MyApplication.gpumod = CudafyModule.TryDeserialize("cudacode.cdfy")
                        Case 2
                            'OpenCL code is device-specific and must be compiled on each initialization
                            'My.MyApplication.gpumod = CudafyModule.TryDeserialize("openclcode.cdfy")
                    End Select
                    If My.MyApplication.gpumod Is Nothing OrElse Not My.MyApplication.gpumod.TryVerifyChecksums() Then
                        Select Case My.Settings.CudafyTarget
                            Case 0, 1
                                Dim cp As New Cudafy.CompileProperties()
                                With cp
                                    .Architecture = eArchitecture.sm_20
                                    .CompileMode = eCudafyCompileMode.Default
                                    .Platform = ePlatform.x86
                                    .WorkingDirectory = My.Computer.FileSystem.SpecialDirectories.Temp
                                    'CUDA SDK v5.0 path
                                    .CompilerPath = "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v5.0\bin\nvcc.exe"
                                    .IncludeDirectoryPath = "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v5.0\include"
                                End With
                                My.MyApplication.gpumod = CudafyTranslator.Cudafy(cp, GetType(DWSIM.SimulationObjects.PropertyPackages.Auxiliary.LeeKeslerPlocker), _
                                            GetType(DWSIM.SimulationObjects.PropertyPackages.ThermoPlugs.PR), _
                                            GetType(DWSIM.SimulationObjects.PropertyPackages.Auxiliary.Unifac), _
                                            GetType(DWSIM.MathEx.Broyden))
                                My.MyApplication.gpumod.Serialize("cudacode.cdfy")
                            Case 2
                                My.MyApplication.gpumod = CudafyTranslator.Cudafy(GetType(DWSIM.SimulationObjects.PropertyPackages.Auxiliary.LeeKeslerPlocker), _
                                           GetType(DWSIM.SimulationObjects.PropertyPackages.ThermoPlugs.PR), _
                                           GetType(DWSIM.SimulationObjects.PropertyPackages.Auxiliary.Unifac), _
                                           GetType(DWSIM.MathEx.Broyden))
                                'My.MyApplication.gpumod.Serialize("openclcode.cdfy")
                        End Select
                    End If
                End If

                'load cudafy module

                If Not My.MyApplication.gpu.IsModuleLoaded(My.MyApplication.gpumod.Name) Then My.MyApplication.gpu.LoadModule(My.MyApplication.gpumod)

            End If

        End Sub

        Shared Function GetGitHash() As String
            Dim gitVersion As String = [String].Empty
            Using stream As Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DWSIM." + "version.txt")
                Using reader As New StreamReader(stream)
                    gitVersion = reader.ReadToEnd()
                End Using
            End Using
            Return gitVersion.Trim
        End Function

    End Class

End Namespace
