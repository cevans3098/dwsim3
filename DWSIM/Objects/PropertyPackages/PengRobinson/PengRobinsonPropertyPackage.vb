﻿'    Peng-Robinson Property Package 
'    Copyright 2008 Daniel Wagner O. de Medeiros
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

Imports DWSIM.DWSIM.SimulationObjects.PropertyPackages
Imports System.Math
Imports DWSIM.DWSIM.SimulationObjects.PropertyPackages.Auxiliary.FlashAlgorithms
Imports DWSIM.DWSIM.ClassesBasicasTermodinamica
Imports System.Threading.Tasks
Imports System.Linq

Namespace DWSIM.SimulationObjects.PropertyPackages

    <System.Runtime.InteropServices.Guid(PengRobinsonPropertyPackage.ClassId)> _
 <System.Serializable()> Public Class PengRobinsonPropertyPackage

        Inherits DWSIM.SimulationObjects.PropertyPackages.PropertyPackage

        Public Shadows Const ClassId As String = "2A322AB7-2256-495d-86C7-797AD19FDE22"

        Public MAT_KIJ(38, 38) As Object

        Private m_props As New DWSIM.SimulationObjects.PropertyPackages.Auxiliary.PROPS
        Public m_pr As New DWSIM.SimulationObjects.PropertyPackages.Auxiliary.PengRobinson

        Public Sub New(ByVal comode As Boolean)
            MyBase.New(comode)
            'With Me.Parameters
            '    .Add("PP_USE_EOS_LIQDENS", 0)
            '    .Add("PP_USE_EOS_VOLUME_SHIFT", 0)
            'End With
        End Sub

        Public Sub New()

            MyBase.New()

            'With Me.Parameters
            '    .Add("PP_USE_EOS_LIQDENS", 0)
            '    .Add("PP_USE_EOS_VOLUME_SHIFT", 0)
            'End With

            Me.IsConfigurable = True
            Me.ConfigForm = New FormConfigPP
            Me._packagetype = PropertyPackages.PackageType.EOS

        End Sub

        Public Overrides Sub ConfigParameters()
            m_par = New System.Collections.Generic.Dictionary(Of String, Double)
            With Me.Parameters
                .Clear()
                .Add("PP_PHFILT", 0.001)
                .Add("PP_PSFILT", 0.001)
                .Add("PP_PHFELT", 0.001)
                .Add("PP_PSFELT", 0.001)
                .Add("PP_PHFMEI", 50)
                .Add("PP_PSFMEI", 50)
                .Add("PP_PHFMII", 100)
                .Add("PP_PSFMII", 100)
                .Add("PP_PTFMEI", 100)
                .Add("PP_PTFMII", 100)
                .Add("PP_PTFILT", 0.001)
                .Add("PP_PTFELT", 0.001)
                .Add("PP_FLASHALGORITHM", 2)
                .Add("PP_FLASHALGORITHMFASTMODE", 1)
                .Add("PP_IDEAL_MIXRULE_LIQDENS", 0)
                .Add("PP_USEEXPLIQDENS", 0)
                .Add("PP_USE_EOS_LIQDENS", 0)
                .Add("PP_USE_EOS_VOLUME_SHIFT", 0)
            End With
        End Sub

        Public Overrides Sub ReconfigureConfigForm()
            MyBase.ReconfigureConfigForm()
            Me.ConfigForm = New FormConfigPP
        End Sub

#Region "    DWSIM Functions"

        Public Overrides Function DW_CalcCp_ISOL(ByVal fase1 As DWSIM.SimulationObjects.PropertyPackages.Fase, ByVal T As Double, ByVal P As Double) As Double
            Select Case fase1
                Case Fase.Liquid
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(fase1), RET_VKij(), RET_VMAS(fase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(1)
                Case Fase.Aqueous
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(fase1), RET_VKij(), RET_VMAS(fase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(1)
                Case Fase.Liquid1
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(fase1), RET_VKij(), RET_VMAS(fase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(1)
                Case Fase.Liquid2
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(fase1), RET_VKij(), RET_VMAS(fase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(1)
                Case Fase.Liquid3
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(fase1), RET_VKij(), RET_VMAS(fase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(1)
                Case Fase.Vapor
                    Return Me.m_props.CpCvR("V", T, P, RET_VMOL(fase1), RET_VKij(), RET_VMAS(fase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(1)
            End Select
        End Function

        Public Overrides Function DW_CalcEnergiaMistura_ISOL(ByVal T As Double, ByVal P As Double) As Double

            Dim HM, HV, HL As Double

            HL = Me.m_pr.H_PR_MIX("L", T, P, RET_VMOL(Fase.Liquid), RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, Me.RET_Hid(298.15, T, Fase.Liquid))
            HV = Me.m_pr.H_PR_MIX("V", T, P, RET_VMOL(Fase.Vapor), RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, Me.RET_Hid(298.15, T, Fase.Vapor))
            HM = Me.CurrentMaterialStream.Fases(1).SPMProperties.massfraction.GetValueOrDefault * HL + Me.CurrentMaterialStream.Fases(2).SPMProperties.massfraction.GetValueOrDefault * HV

            Dim ent_massica = HM
            Dim flow = Me.CurrentMaterialStream.Fases(0).SPMProperties.massflow
            Return ent_massica * flow

        End Function

        Public Overrides Function DW_CalcK_ISOL(ByVal fase1 As DWSIM.SimulationObjects.PropertyPackages.Fase, ByVal T As Double, ByVal P As Double) As Double
            If fase1 = Fase.Liquid Then
                Return Me.AUX_CONDTL(T)
            ElseIf fase1 = Fase.Vapor Then
                Return Me.AUX_CONDTG(T, P)
            End If
        End Function

        Public Overrides Function DW_CalcMassaEspecifica_ISOL(ByVal fase1 As DWSIM.SimulationObjects.PropertyPackages.Fase, ByVal T As Double, ByVal P As Double, Optional ByVal pvp As Double = 0) As Double
            If fase1 = Fase.Liquid Then
                Return Me.AUX_LIQDENS(T)
            ElseIf fase1 = Fase.Vapor Then
                Return Me.AUX_VAPDENS(T, P)
            ElseIf fase1 = Fase.Mixture Then
                Return Me.CurrentMaterialStream.Fases(1).SPMProperties.volumetric_flow.GetValueOrDefault * Me.AUX_LIQDENS(T) / Me.CurrentMaterialStream.Fases(0).SPMProperties.volumetric_flow.GetValueOrDefault + Me.CurrentMaterialStream.Fases(2).SPMProperties.volumetric_flow.GetValueOrDefault * Me.AUX_VAPDENS(T, P) / Me.CurrentMaterialStream.Fases(0).SPMProperties.volumetric_flow.GetValueOrDefault
            End If
        End Function

        Public Overrides Function DW_CalcMM_ISOL(ByVal fase1 As DWSIM.SimulationObjects.PropertyPackages.Fase, ByVal T As Double, ByVal P As Double) As Double
            Return Me.AUX_MMM(fase1)
        End Function

        Public Overrides Sub DW_CalcOverallProps()
            MyBase.DW_CalcOverallProps()
        End Sub

        Public Overrides Sub DW_CalcProp(ByVal [property] As String, ByVal phase As Fase)

            Dim result As Double = 0.0#
            Dim resultObj As Object = Nothing
            Dim phaseID As Integer = -1
            Dim state As String = ""

            Dim T, P As Double
            T = Me.CurrentMaterialStream.Fases(0).SPMProperties.temperature.GetValueOrDefault
            P = Me.CurrentMaterialStream.Fases(0).SPMProperties.pressure.GetValueOrDefault

            Select Case phase
                Case Fase.Vapor
                    state = "V"
                Case Fase.Liquid, Fase.Liquid1, Fase.Liquid2, Fase.Liquid3
                    state = "L"
            End Select

            Select Case phase
                Case PropertyPackages.Fase.Mixture
                    phaseID = 0
                Case PropertyPackages.Fase.Vapor
                    phaseID = 2
                Case PropertyPackages.Fase.Liquid1
                    phaseID = 3
                Case PropertyPackages.Fase.Liquid2
                    phaseID = 4
                Case PropertyPackages.Fase.Liquid3
                    phaseID = 5
                Case PropertyPackages.Fase.Liquid
                    phaseID = 1
                Case PropertyPackages.Fase.Aqueous
                    phaseID = 6
                Case PropertyPackages.Fase.Solid
                    phaseID = 7
            End Select

            Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molecularWeight = Me.AUX_MMM(phase)

            Select Case [property].ToLower
                Case "compressibilityfactor"
                    result = m_pr.Z_PR(T, P, RET_VMOL(phase), RET_VKij(), RET_VTC, RET_VPC, RET_VW, state)
                    MsgBox(result)
                    If CInt(Me.Parameters("PP_USE_EOS_VOLUME_SHIFT")) = 1 Then
                        result -= Me.AUX_CM(phase) / 8.314 / T * P
                    End If
                    MsgBox(result)
                    Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.compressibilityFactor = result
                Case "heatcapacity", "heatcapacitycp"
                    resultObj = Me.m_props.CpCvR(state, T, P, RET_VMOL(phase), RET_VKij(), RET_VMAS(phase), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())
                    Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.heatCapacityCp = resultObj(1)
                Case "heatcapacitycv"
                    resultObj = Me.m_props.CpCvR(state, T, P, RET_VMOL(phase), RET_VKij(), RET_VMAS(phase), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())
                    Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.heatCapacityCv = resultObj(2)
                Case "enthalpy", "enthalpynf"
                    result = Me.m_pr.H_PR_MIX(state, T, P, RET_VMOL(phase), RET_VKij, RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), Me.RET_Hid(298.15, T, phase))
                    Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.enthalpy = result
                    result = Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.enthalpy.GetValueOrDefault * Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molecularWeight.GetValueOrDefault
                    Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molar_enthalpy = result
                Case "entropy", "entropynf"
                    result = Me.m_pr.S_PR_MIX(state, T, P, RET_VMOL(phase), RET_VKij, RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), Me.RET_Sid(298.15, T, P, phase))
                    Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.entropy = result
                    result = Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.entropy.GetValueOrDefault * Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molecularWeight.GetValueOrDefault
                    Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molar_entropy = result
                Case "excessenthalpy"
                    result = Me.m_pr.H_PR_MIX(state, T, P, RET_VMOL(phase), RET_VKij, RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), 0)
                    Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.excessEnthalpy = result
                Case "excessentropy"
                    result = Me.m_pr.S_PR_MIX(state, T, P, RET_VMOL(phase), RET_VKij, RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), 0)
                    Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.excessEntropy = result
                Case "enthalpyf"
                    Dim entF As Double = Me.AUX_HFm25(phase)
                    result = Me.m_pr.H_PR_MIX(state, T, P, RET_VMOL(phase), RET_VKij, RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), Me.RET_Hid(298.15, T, phase))
                    Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.enthalpyF = result + entF
                    result = Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.enthalpyF.GetValueOrDefault * Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molecularWeight.GetValueOrDefault
                    Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molar_enthalpyF = result
                Case "entropyf"
                    Dim entF As Double = Me.AUX_SFm25(phase)
                    result = Me.m_pr.S_PR_MIX(state, T, P, RET_VMOL(phase), RET_VKij, RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), Me.RET_Sid(298.15, T, P, phase))
                    Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.entropyF = result + entF
                    result = Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.entropyF.GetValueOrDefault * Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molecularWeight.GetValueOrDefault
                    Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molar_entropyF = result
                Case "viscosity"
                    If state = "L" Then
                        result = Me.AUX_LIQVISCm(T)
                    Else
                        result = Me.AUX_VAPVISCm(T, Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.density.GetValueOrDefault, Me.AUX_MMM(phase))
                    End If
                    Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.viscosity = result
                Case "thermalconductivity"
                    If state = "L" Then
                        result = Me.AUX_CONDTL(T)
                    Else
                        result = Me.AUX_CONDTG(T, P)
                    End If
                    Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.thermalConductivity = result
                Case "fugacity", "fugacitycoefficient", "logfugacitycoefficient", "activity", "activitycoefficient"
                    Me.DW_CalcCompFugCoeff(phase)
                Case "volume", "density"
                    If state = "L" Then
                        result = Me.AUX_LIQDENS(T, P, 0.0#, phaseID, False)
                    Else
                        result = Me.AUX_VAPDENS(T, P)
                    End If
                    Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.density = result
                Case "surfacetension"
                    Me.CurrentMaterialStream.Fases(0).TPMProperties.surfaceTension = Me.AUX_SURFTM(T)
                Case Else
                    Dim ex As Exception = New CapeOpen.CapeThrmPropertyNotAvailableException
                    ThrowCAPEException(ex, "Error", ex.Message, "ICapeThermoMaterial", ex.Source, ex.StackTrace, "CalcSinglePhaseProp/CalcTwoPhaseProp/CalcProp", ex.GetHashCode)
            End Select

        End Sub

        Public Overrides Sub DW_CalcPhaseProps(ByVal fase As DWSIM.SimulationObjects.PropertyPackages.Fase)

            Dim result As Double
            Dim resultObj As Object
            Dim dwpl As Fase

            Dim T, P As Double
            Dim phasemolarfrac As Double = Nothing
            Dim overallmolarflow As Double = Nothing

            Dim phaseID As Integer
            T = Me.CurrentMaterialStream.Fases(0).SPMProperties.temperature.GetValueOrDefault
            P = Me.CurrentMaterialStream.Fases(0).SPMProperties.pressure.GetValueOrDefault

            Select Case fase
                Case PropertyPackages.Fase.Mixture
                    phaseID = 0
                    dwpl = PropertyPackages.Fase.Mixture
                Case PropertyPackages.Fase.Vapor
                    phaseID = 2
                    dwpl = PropertyPackages.Fase.Vapor
                Case PropertyPackages.Fase.Liquid1
                    phaseID = 3
                    dwpl = PropertyPackages.Fase.Liquid1
                Case PropertyPackages.Fase.Liquid2
                    phaseID = 4
                    dwpl = PropertyPackages.Fase.Liquid2
                Case PropertyPackages.Fase.Liquid3
                    phaseID = 5
                    dwpl = PropertyPackages.Fase.Liquid3
                Case PropertyPackages.Fase.Liquid
                    phaseID = 1
                    dwpl = PropertyPackages.Fase.Liquid
                Case PropertyPackages.Fase.Aqueous
                    phaseID = 6
                    dwpl = PropertyPackages.Fase.Aqueous
                Case PropertyPackages.Fase.Solid
                    phaseID = 7
                    dwpl = PropertyPackages.Fase.Solid
            End Select

            If phaseID > 0 Then

                overallmolarflow = Me.CurrentMaterialStream.Fases(0).SPMProperties.molarflow.GetValueOrDefault
                phasemolarfrac = Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molarfraction.GetValueOrDefault
                result = overallmolarflow * phasemolarfrac
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molarflow = result
                result = result * Me.AUX_MMM(fase) / 1000
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.massflow = result
                result = phasemolarfrac * overallmolarflow * Me.AUX_MMM(fase) / 1000 / Me.CurrentMaterialStream.Fases(0).SPMProperties.massflow.GetValueOrDefault
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.massfraction = result
                Me.DW_CalcCompVolFlow(phaseID)
                Me.DW_CalcCompFugCoeff(fase)

            End If

            If Not Me.Parameters.ContainsKey("PP_USE_EOS_VOLUME_SHIFT") Then Me.Parameters.Add("PP_USE_EOS_VOLUME_SHIFT", 0)

            If phaseID = 3 Or phaseID = 4 Or phaseID = 5 Or phaseID = 6 Then

                If CInt(Me.Parameters("PP_USE_EOS_LIQDENS")) = 1 Then
                    Dim val As Double
                    val = m_pr.Z_PR(T, P, RET_VMOL(fase), RET_VKij(), RET_VTC, RET_VPC, RET_VW, "L")
                    val = (8.314 * val * T / P)
                    If CInt(Me.Parameters("PP_USE_EOS_VOLUME_SHIFT")) = 1 Then
                        val -= Me.AUX_CM(fase)
                    End If
                    val = 1 / val * Me.AUX_MMM(dwpl) / 1000
                    result = val
                Else
                    result = Me.AUX_LIQDENS(T, P, 0.0#, phaseID, False)
                End If

                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.density = result

                result = Me.m_pr.H_PR_MIX("L", T, P, RET_VMOL(dwpl), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), Me.RET_Hid(298.15, T, dwpl))
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.enthalpy = result
                result = Me.m_pr.S_PR_MIX("L", T, P, RET_VMOL(dwpl), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), Me.RET_Sid(298.15, T, P, dwpl))
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.entropy = result
                result = Me.m_pr.Z_PR(T, P, RET_VMOL(dwpl), RET_VKij(), RET_VTC, RET_VPC, RET_VW, "L")
                If CInt(Me.Parameters("PP_USE_EOS_VOLUME_SHIFT")) = 1 Then
                    result -= Me.AUX_CM(dwpl) / 8.314 / T * P
                End If
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.compressibilityFactor = result
                resultObj = Me.m_props.CpCvR("L", T, P, RET_VMOL(dwpl), RET_VKij(), RET_VMAS(dwpl), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.heatCapacityCp = resultObj(1)
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.heatCapacityCv = resultObj(2)
                result = Me.AUX_MMM(fase)
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molecularWeight = result
                result = Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.enthalpy.GetValueOrDefault * Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molecularWeight.GetValueOrDefault
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molar_enthalpy = result
                result = Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.entropy.GetValueOrDefault * Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molecularWeight.GetValueOrDefault
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molar_entropy = result
                result = Me.AUX_CONDTL(T)
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.thermalConductivity = result
                result = Me.AUX_LIQVISCm(T)
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.viscosity = result
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.kinematic_viscosity = result / Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.density.GetValueOrDefault

            ElseIf phaseID = 2 Then

                result = Me.AUX_VAPDENS(T, P)
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.density = result
                result = Me.m_pr.H_PR_MIX("V", T, P, RET_VMOL(fase.Vapor), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), Me.RET_Hid(298.15, T, fase.Vapor))
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.enthalpy = result
                result = Me.m_pr.S_PR_MIX("V", T, P, RET_VMOL(fase.Vapor), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VMM(), Me.RET_Sid(298.15, T, P, fase.Vapor))
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.entropy = result
                result = Me.m_pr.Z_PR(T, P, RET_VMOL(fase.Vapor), RET_VKij, RET_VTC, RET_VPC, RET_VW, "V")
                If CInt(Me.Parameters("PP_USE_EOS_VOLUME_SHIFT")) = 1 Then
                    result -= Me.AUX_CM(fase.Vapor) / 8.314 / T * P
                End If
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.compressibilityFactor = result
                result = Me.AUX_CPm(PropertyPackages.Fase.Vapor, T)
                resultObj = Me.m_props.CpCvR("V", T, P, RET_VMOL(PropertyPackages.Fase.Vapor), RET_VKij(), RET_VMAS(PropertyPackages.Fase.Vapor), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.heatCapacityCp = resultObj(1)
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.heatCapacityCv = resultObj(2)
                result = Me.AUX_MMM(fase)
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molecularWeight = result
                result = Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.enthalpy.GetValueOrDefault * Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molecularWeight.GetValueOrDefault
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molar_enthalpy = result
                result = Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.entropy.GetValueOrDefault * Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molecularWeight.GetValueOrDefault
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.molar_entropy = result
                result = Me.AUX_CONDTG(T, P)
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.thermalConductivity = result
                result = Me.AUX_VAPVISCm(T, Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.density.GetValueOrDefault, Me.AUX_MMM(fase))
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.viscosity = result
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.kinematic_viscosity = result / Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.density.GetValueOrDefault

            ElseIf phaseID = 1 Then

                DW_CalcLiqMixtureProps()


            Else

                DW_CalcOverallProps()

            End If


            If phaseID > 0 Then
                result = overallmolarflow * phasemolarfrac * Me.AUX_MMM(fase) / 1000 / Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.density.GetValueOrDefault
                Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.volumetric_flow = result
            Else
                'result = Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.massflow.GetValueOrDefault / Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.density.GetValueOrDefault
                'Me.CurrentMaterialStream.Fases(phaseID).SPMProperties.volumetric_flow = result
            End If


        End Sub

        Public Overrides Function DW_CalcPVAP_ISOL(ByVal T As Double) As Double
            Return Me.m_props.Pvp_leekesler(T, Me.RET_VTC(Fase.Liquid), Me.RET_VPC(Fase.Liquid), Me.RET_VW(Fase.Liquid))
        End Function

        Public Overrides Function DW_CalcTensaoSuperficial_ISOL(ByVal fase1 As DWSIM.SimulationObjects.PropertyPackages.Fase, ByVal T As Double, ByVal P As Double) As Double
            Return Me.AUX_SURFTM(T)
        End Function

        Public Overrides Sub DW_CalcTwoPhaseProps(ByVal fase1 As DWSIM.SimulationObjects.PropertyPackages.Fase, ByVal fase2 As DWSIM.SimulationObjects.PropertyPackages.Fase)

            Dim T As Double

            T = Me.CurrentMaterialStream.Fases(0).SPMProperties.temperature.GetValueOrDefault
            Me.CurrentMaterialStream.Fases(0).TPMProperties.surfaceTension = Me.AUX_SURFTM(T)

        End Sub

        Public Overrides Sub DW_CalcVazaoMassica()
            With Me.CurrentMaterialStream
                .Fases(0).SPMProperties.massflow = .Fases(0).SPMProperties.molarflow.GetValueOrDefault * Me.AUX_MMM(Fase.Mixture) / 1000
            End With
        End Sub

        Public Overrides Sub DW_CalcVazaoMolar()
            With Me.CurrentMaterialStream
                .Fases(0).SPMProperties.molarflow = .Fases(0).SPMProperties.massflow.GetValueOrDefault / Me.AUX_MMM(Fase.Mixture) * 1000
            End With
        End Sub

        Public Overrides Sub DW_CalcVazaoVolumetrica()
            With Me.CurrentMaterialStream
                .Fases(0).SPMProperties.volumetric_flow = .Fases(0).SPMProperties.massflow.GetValueOrDefault / .Fases(0).SPMProperties.density.GetValueOrDefault
            End With
        End Sub

        Public Overrides Function DW_CalcViscosidadeDinamica_ISOL(ByVal fase1 As DWSIM.SimulationObjects.PropertyPackages.Fase, ByVal T As Double, ByVal P As Double) As Double
            If fase1 = Fase.Liquid Then
                Return Me.AUX_LIQVISCm(T)
            ElseIf fase1 = Fase.Vapor Then
                Return Me.AUX_VAPVISCm(T, Me.AUX_VAPDENS(T, P), Me.AUX_MMM(Fase.Vapor))
            End If
        End Function

        Public Overrides Function SupportsComponent(ByVal comp As ClassesBasicasTermodinamica.ConstantProperties) As Boolean

            Return True

        End Function

        Public Overrides Function DW_ReturnPhaseEnvelope(ByVal parameters As Object, Optional ByVal bw As System.ComponentModel.BackgroundWorker = Nothing) As Object

            If My.Settings.EnableGPUProcessing Then DWSIM.App.InitComputeDevice()

            If My.Settings.EnableParallelProcessing Then
                Return DW_ReturnPhaseEnvelopeParallel(parameters, bw)
            Else
                Return DW_ReturnPhaseEnvelopeSequential(parameters, bw)
            End If

        End Function

        Public Function DW_ReturnPhaseEnvelopeSequential(ByVal parameters As Object, Optional ByVal bw As System.ComponentModel.BackgroundWorker = Nothing) As Object

            Dim cpc As New DWSIM.Utilities.TCP.Methods

            Dim i As Integer

            Dim n As Integer = Me.CurrentMaterialStream.Fases(0).Componentes.Count - 1

            Dim Vz(n) As Double
            Dim comp As DWSIM.ClassesBasicasTermodinamica.Substancia

            i = 0
            For Each comp In Me.CurrentMaterialStream.Fases(0).Componentes.Values
                Vz(i) += comp.FracaoMolar.GetValueOrDefault
                i += 1
            Next

            Dim j, k, l As Integer
            i = 0
            Do
                If Vz(i) = 0 Then j += 1
                i = i + 1
            Loop Until i = n + 1

            Dim VTc(n), Vpc(n), Vw(n), VVc(n), VKij(n, n) As Double
            Dim Vm2(UBound(Vz) - j), VPc2(UBound(Vz) - j), VTc2(UBound(Vz) - j), VVc2(UBound(Vz) - j), Vw2(UBound(Vz) - j), VKij2(UBound(Vz) - j, UBound(Vz) - j)

            VTc = Me.RET_VTC()
            Vpc = Me.RET_VPC()
            VVc = Me.RET_VVC()
            Vw = Me.RET_VW()
            VKij = Me.RET_VKij

            i = 0
            k = 0
            Do
                If Vz(i) <> 0 Then
                    Vm2(k) = Vz(i)
                    VTc2(k) = VTc(i)
                    VPc2(k) = Vpc(i)
                    VVc2(k) = VVc(i)
                    Vw2(k) = Vw(i)
                    j = 0
                    l = 0
                    Do
                        If Vz(l) <> 0 Then
                            VKij2(k, j) = VKij(i, l)
                            j = j + 1
                        End If
                        l = l + 1
                    Loop Until l = n + 1
                    k = k + 1
                End If
                i = i + 1
            Loop Until i = n + 1

            Dim Pmin, Tmin, dP, dT, T, P As Double
            Dim PB, PO, TVB, TVD, HB, HO, SB, SO, VB, VO, TE, PE, TH, PHsI, PHsII, TQ, PQ, TI, PI, POWF, TOWF, HOWF, SOWF, VOWF As New ArrayList
            Dim TCR, PCR, VCR As Double

            Dim CP As New ArrayList
            If n > 0 Then
                CP = cpc.CRITPT_PR(Vm2, VTc2, VPc2, VVc2, Vw2, VKij2)
                If CP.Count > 0 Then
                    Dim cp0 = CP(0)
                    TCR = cp0(0)
                    PCR = cp0(1)
                    VCR = cp0(2)
                Else
                    TCR = Me.AUX_TCM(Fase.Mixture)
                    PCR = Me.AUX_PCM(Fase.Mixture)
                    VCR = Me.AUX_VCM(Fase.Mixture)
                End If
            Else
                TCR = Me.AUX_TCM(Fase.Mixture)
                PCR = Me.AUX_PCM(Fase.Mixture)
                VCR = Me.AUX_VCM(Fase.Mixture)
            End If
            CP.Add(New Object() {TCR, PCR, VCR})

            Pmin = 101325
            Tmin = 0.3 * TCR

            dP = (PCR - Pmin) / 50
            dT = (TCR - Tmin) / 50

            Dim beta As Double = 10

            Dim tmp2 As Object
            Dim KI(n) As Double

            j = 0
            Do
                KI(j) = 0
                j = j + 1
            Loop Until j = n + 1

            i = 0
            P = Pmin
            T = Tmin
            Do
                If i < 2 Then
                    Try
                        tmp2 = Me.FlashBase.Flash_PV(Me.RET_VMOL(Fase.Mixture), P, 0, 0, Me)
                        TVB.Add(tmp2(4))
                        PB.Add(P)
                        T = TVB(i)
                        HB.Add(Me.DW_CalcEnthalpy(Me.RET_VMOL(Fase.Mixture), T, P, State.Liquid))
                        SB.Add(Me.DW_CalcEntropy(Me.RET_VMOL(Fase.Mixture), T, P, State.Liquid))
                        VB.Add(1 / Me.AUX_LIQDENS(T, P, 0, 0, False) * Me.AUX_MMM(Fase.Mixture))
                        KI = tmp2(6)
                    Catch ex As Exception

                    Finally
                        P = P + dP
                    End Try
                Else
                    If beta < 20 Then
                        Try
                            tmp2 = Me.FlashBase.Flash_TV(Me.RET_VMOL(Fase.Mixture), T, 0, PB(i - 1), Me, True, KI)
                            TVB.Add(T)
                            PB.Add(tmp2(4))
                            P = PB(i)
                            HB.Add(Me.DW_CalcEnthalpy(Me.RET_VMOL(Fase.Mixture), T, P, State.Liquid))
                            SB.Add(Me.DW_CalcEntropy(Me.RET_VMOL(Fase.Mixture), T, P, State.Liquid))
                            VB.Add(1 / Me.AUX_LIQDENS(T, P, 0, 0, False) * Me.AUX_MMM(Fase.Mixture))
                            KI = tmp2(6)
                        Catch ex As Exception

                        Finally
                            If Math.Abs(T - TCR) / TCR < 0.01 And Math.Abs(P - PCR) / PCR < 0.02 Then
                                T = T + dT * 0.5
                            Else
                                T = T + dT
                            End If
                        End Try
                    Else
                        Try
                            tmp2 = Me.FlashBase.Flash_PV(Me.RET_VMOL(Fase.Mixture), P, 0, TVB(i - 1), Me, True, KI)
                            TVB.Add(tmp2(4))
                            PB.Add(P)
                            T = TVB(i)
                            HB.Add(Me.DW_CalcEnthalpy(Me.RET_VMOL(Fase.Mixture), T, P, State.Liquid))
                            SB.Add(Me.DW_CalcEntropy(Me.RET_VMOL(Fase.Mixture), T, P, State.Liquid))
                            VB.Add(1 / Me.AUX_LIQDENS(T, P, 0, 0, False) * Me.AUX_MMM(Fase.Mixture))
                            KI = tmp2(6)
                        Catch ex As Exception

                        Finally
                            If Math.Abs(T - TCR) / TCR < 0.01 And Math.Abs(P - PCR) / PCR < 0.01 Then
                                P = P + dP * 0.1
                            Else
                                P = P + dP
                            End If
                        End Try
                    End If
                    beta = (Math.Log(PB(i) / 101325) - Math.Log(PB(i - 1) / 101325)) / (Math.Log(TVB(i)) - Math.Log(TVB(i - 1)))
                End If
                If bw IsNot Nothing Then If bw.CancellationPending Then Exit Do Else bw.ReportProgress(0, "Bubble Points (" & i + 1 & "/200)")
                i = i + 1
            Loop Until i >= 200 Or PB(i - 1) = 0 Or PB(i - 1) < 0 Or TVB(i - 1) < 0 Or _
                        T >= TCR Or Double.IsNaN(PB(i - 1)) = True Or _
                        Double.IsNaN(TVB(i - 1)) = True Or Math.Abs(T - TCR) / TCR < 0.002 And _
                        Math.Abs(P - PCR) / PCR < 0.002

            If Me.RET_VCAS().Contains("7732-18-5") Then

                Dim wi As Integer = Array.IndexOf(Me.RET_VCAS(), "7732-18-5")
                Dim wmf As Double = Vz(wi)
                Dim Vzwf(n) As Double
                For i = 0 To n
                    If i <> wi Then
                        Vzwf(i) = Vz(i) / (1 - wmf)
                    Else
                        Vzwf(i) = 0.0#
                    End If
                Next

                beta = 10

                j = 0
                Do
                    KI(j) = 0
                    j = j + 1
                Loop Until j = n + 1

                'water-free calc
                i = 0
                P = Pmin
                Do
                    If i < 2 Then
                        Try
                            tmp2 = Me.FlashBase.Flash_PV(Vzwf, P, 1, 0, Me)
                            TOWF.Add(tmp2(4))
                            POWF.Add(P)
                            T = TOWF(i)
                            HOWF.Add(Me.DW_CalcEnthalpy(Vzwf, T, P, State.Vapor))
                            SOWF.Add(Me.DW_CalcEntropy(Vzwf, T, P, State.Vapor))
                            VOWF.Add(1 / Me.AUX_VAPDENS(T, P) * Me.AUX_MMM(Vzwf))
                            KI = tmp2(6)
                        Catch ex As Exception
                        Finally
                            If Math.Abs(T - TCR) / TCR < 0.01 And Math.Abs(P - PCR) / PCR < 0.01 Then
                                P = P + dP * 0.1
                            Else
                                P = P + dP
                            End If
                        End Try
                    Else
                        If Abs(beta) < 2 Then
                            Try
                                tmp2 = Me.FlashBase.Flash_TV(Vzwf, T, 1, POWF(i - 1), Me, True, KI)
                                TOWF.Add(T)
                                POWF.Add(tmp2(4))
                                P = POWF(i)
                                HOWF.Add(Me.DW_CalcEnthalpy(Vzwf, T, P, State.Vapor))
                                SOWF.Add(Me.DW_CalcEntropy(Vzwf, T, P, State.Vapor))
                                VOWF.Add(1 / Me.AUX_VAPDENS(T, P) * Me.AUX_MMM(Vzwf))
                                KI = tmp2(6)
                            Catch ex As Exception
                            Finally
                                If TOWF(i) - TOWF(i - 1) <= 0 Then
                                    If Math.Abs(T - TCR) / TCR < 0.02 And Math.Abs(P - PCR) / PCR < 0.02 Then
                                        T = T - dT * 0.1
                                    Else
                                        T = T - dT
                                    End If
                                Else
                                    If Math.Abs(T - TCR) / TCR < 0.02 And Math.Abs(P - PCR) / PCR < 0.02 Then
                                        T = T + dT * 0.1
                                    Else
                                        T = T + dT
                                    End If
                                End If
                            End Try
                        Else
                            Try
                                tmp2 = Me.FlashBase.Flash_PV(Vzwf, P, 1, TOWF(i - 1), Me, False, KI)
                                TOWF.Add(tmp2(4))
                                POWF.Add(P)
                                T = TOWF(i)
                                HOWF.Add(Me.DW_CalcEnthalpy(Vzwf, T, P, State.Vapor))
                                SOWF.Add(Me.DW_CalcEntropy(Vzwf, T, P, State.Vapor))
                                VOWF.Add(1 / Me.AUX_VAPDENS(T, P) * Me.AUX_MMM(Vzwf))
                                KI = tmp2(6)
                            Catch ex As Exception
                            Finally
                                If Math.Abs(T - TCR) / TCR < 0.05 And Math.Abs(P - PCR) / PCR < 0.05 Then
                                    P = P + dP * 0.25
                                Else
                                    P = P + dP
                                End If
                            End Try
                        End If
                        If i >= POWF.Count Then
                            i = i - 1
                        End If
                        beta = (Math.Log(POWF(i) / 101325) - Math.Log(POWF(i - 1) / 101325)) / (Math.Log(TOWF(i)) - Math.Log(TOWF(i - 1)))
                        If Double.IsNaN(beta) Or Double.IsInfinity(beta) Then beta = 0
                    End If
                    If bw IsNot Nothing Then If bw.CancellationPending Then Exit Do Else bw.ReportProgress(0, "Dew Points (Water-Free) (" & i + 1 & "/200)")
                    i = i + 1
                Loop Until i >= 200 Or POWF(i - 1) = 0 Or POWF(i - 1) < 0 Or TOWF(i - 1) < 0 Or _
                            Double.IsNaN(POWF(i - 1)) = True Or Double.IsNaN(TOWF(i - 1)) = True Or _
                            Math.Abs(T - TCR) / TCR < 0.03 And Math.Abs(P - PCR) / PCR < 0.03

            End If

            beta = 10

            j = 0
            Do
                KI(j) = 0
                j = j + 1
            Loop Until j = n + 1

            i = 0
            P = Pmin
            Do
                If i < 2 Then
                    Try
                        tmp2 = Me.FlashBase.Flash_PV(Me.RET_VMOL(Fase.Mixture), P, 1, 0, Me)
                        TVD.Add(tmp2(4))
                        PO.Add(P)
                        T = TVD(i)
                        HO.Add(Me.DW_CalcEnthalpy(Me.RET_VMOL(Fase.Mixture), T, P, State.Vapor))
                        SO.Add(Me.DW_CalcEntropy(Me.RET_VMOL(Fase.Mixture), T, P, State.Vapor))
                        VO.Add(1 / Me.AUX_VAPDENS(T, P) * Me.AUX_MMM(Fase.Mixture))
                        KI = tmp2(6)
                    Catch ex As Exception
                    Finally
                        If Math.Abs(T - TCR) / TCR < 0.01 And Math.Abs(P - PCR) / PCR < 0.01 Then
                            P = P + dP * 0.1
                        Else
                            P = P + dP
                        End If
                    End Try
                Else
                    If Abs(beta) < 2 Then
                        Try
                            tmp2 = Me.FlashBase.Flash_TV(Me.RET_VMOL(Fase.Mixture), T, 1, PO(i - 1), Me, True, KI)
                            TVD.Add(T)
                            PO.Add(tmp2(4))
                            P = PO(i)
                            HO.Add(Me.DW_CalcEnthalpy(Me.RET_VMOL(Fase.Mixture), T, P, State.Vapor))
                            SO.Add(Me.DW_CalcEntropy(Me.RET_VMOL(Fase.Mixture), T, P, State.Vapor))
                            VO.Add(1 / Me.AUX_VAPDENS(T, P) * Me.AUX_MMM(Fase.Mixture))
                            KI = tmp2(6)
                        Catch ex As Exception
                        Finally
                            If TVD(i) - TVD(i - 1) <= 0 Then
                                If Math.Abs(T - TCR) / TCR < 0.02 And Math.Abs(P - PCR) / PCR < 0.02 Then
                                    T = T - dT * 0.1
                                Else
                                    T = T - dT
                                End If
                            Else
                                If Math.Abs(T - TCR) / TCR < 0.02 And Math.Abs(P - PCR) / PCR < 0.02 Then
                                    T = T + dT * 0.1
                                Else
                                    T = T + dT
                                End If
                            End If
                        End Try
                    Else
                        Try
                            tmp2 = Me.FlashBase.Flash_PV(Me.RET_VMOL(Fase.Mixture), P, 1, TVD(i - 1), Me, False, KI)
                            TVD.Add(tmp2(4))
                            PO.Add(P)
                            T = TVD(i)
                            HO.Add(Me.DW_CalcEnthalpy(Me.RET_VMOL(Fase.Mixture), T, P, State.Vapor))
                            SO.Add(Me.DW_CalcEntropy(Me.RET_VMOL(Fase.Mixture), T, P, State.Vapor))
                            VO.Add(1 / Me.AUX_VAPDENS(T, P) * Me.AUX_MMM(Fase.Mixture))
                            KI = tmp2(6)
                        Catch ex As Exception
                        Finally
                            If Math.Abs(T - TCR) / TCR < 0.05 And Math.Abs(P - PCR) / PCR < 0.05 Then
                                P = P + dP * 0.25
                            Else
                                P = P + dP
                            End If
                        End Try
                    End If
                    If i >= PO.Count Then
                        i = i - 1
                    End If
                    beta = (Math.Log(PO(i) / 101325) - Math.Log(PO(i - 1) / 101325)) / (Math.Log(TVD(i)) - Math.Log(TVD(i - 1)))
                    If Double.IsNaN(beta) Or Double.IsInfinity(beta) Then beta = 0
                End If
                If bw IsNot Nothing Then If bw.CancellationPending Then Exit Do Else bw.ReportProgress(0, "Dew Points (" & i + 1 & "/200)")
                i = i + 1
            Loop Until i >= 200 Or PO(i - 1) = 0 Or PO(i - 1) < 0 Or TVD(i - 1) < 0 Or _
                        Double.IsNaN(PO(i - 1)) = True Or Double.IsNaN(TVD(i - 1)) = True Or _
                        Math.Abs(T - TCR) / TCR < 0.03 And Math.Abs(P - PCR) / PCR < 0.03

            beta = 10

            If CBool(parameters(2)) = True Then

                j = 0
                Do
                    KI(j) = 0
                    j = j + 1
                Loop Until j = n + 1

                i = 0
                P = 400000
                T = TVD(0)
                Do
                    If i < 2 Then
                        Try
                            tmp2 = Me.FlashBase.Flash_PV(Me.RET_VMOL(Fase.Mixture), P, parameters(1), 0, Me, False, KI)
                            TQ.Add(tmp2(4))
                            PQ.Add(P)
                            T = TQ(i)
                            KI = tmp2(6)
                        Catch ex As Exception
                        Finally
                            P = P + dP
                        End Try
                    Else
                        If beta < 2 Then
                            Try
                                tmp2 = Me.FlashBase.Flash_TV(Me.RET_VMOL(Fase.Mixture), T, parameters(1), PQ(i - 1), Me, True, KI)
                                TQ.Add(T)
                                PQ.Add(tmp2(4))
                                P = PQ(i)
                                KI = tmp2(6)
                            Catch ex As Exception
                            Finally
                                If Math.Abs(T - TCR) / TCR < 0.1 And Math.Abs(P - PCR) / PCR < 0.2 Then
                                    T = T + dT * 0.25
                                Else
                                    T = T + dT
                                End If
                            End Try
                        Else
                            Try
                                tmp2 = Me.FlashBase.Flash_PV(Me.RET_VMOL(Fase.Mixture), P, parameters(1), TQ(i - 1), Me, True, KI)
                                TQ.Add(tmp2(4))
                                PQ.Add(P)
                                T = TQ(i)
                                KI = tmp2(6)
                            Catch ex As Exception
                            Finally
                                If Math.Abs(T - TCR) / TCR < 0.1 And Math.Abs(P - PCR) / PCR < 0.1 Then
                                    P = P + dP * 0.1
                                Else
                                    P = P + dP
                                End If
                            End Try
                        End If
                        beta = (Math.Log(PQ(i) / 101325) - Math.Log(PQ(i - 1) / 101325)) / (Math.Log(TQ(i)) - Math.Log(TQ(i - 1)))
                    End If
                    If bw IsNot Nothing Then If bw.CancellationPending Then Exit Do Else bw.ReportProgress(0, "Quality Line (" & i + 1 & "/200)")
                    i = i + 1
                    If i > 2 Then
                        If PQ(i - 1) = PQ(i - 2) Or TQ(i - 1) = TQ(i - 2) Then Exit Do
                    End If
                Loop Until i >= 200 Or PQ(i - 1) = 0 Or PQ(i - 1) < 0 Or TQ(i - 1) < 0 Or _
                            Double.IsNaN(PQ(i - 1)) = True Or Double.IsNaN(TQ(i - 1)) = True Or _
                            Math.Abs(T - TCR) / TCR < 0.02 And Math.Abs(P - PCR) / PCR < 0.02
            Else
                TQ.Add(0)
                PQ.Add(0)
            End If

            If n > 0 And CBool(parameters(3)) = True Then
                If bw IsNot Nothing Then If bw.CancellationPending Then bw.ReportProgress(0, "Stability Line")
                Dim res As ArrayList = cpc.STABILITY_CURVE(Vm2, VTc2, VPc2, VVc2, Vw2, VKij2)
                i = 0
                Do
                    TE.Add(res(i)(0))
                    PE.Add(res(i)(1))
                    i += 1
                Loop Until i = res.Count
            Else
                TE.Add(0)
                PE.Add(0)
            End If

            Dim Pest, Tmax As Double

            Pest = PCR * 10
            Tmin = MathEx.Common.Max(Me.RET_VTF)
            If Tmin = 0.0# Then Tmin = MathEx.Common.Min(Me.RET_VTB) * 0.4
            Tmax = TCR * 1.4

            PI.Clear()
            TI.Clear()

            If CBool(parameters(4)) = True Then
                If bw IsNot Nothing Then bw.ReportProgress(0, "Phase Identification Parameter")
                For T = Tmin To Tmax Step 5
                    TI.Add(T)
                    PI.Add(Auxiliary.FlashAlgorithms.FlashAlgorithm.CalcPIPressure(Vz, Pest, T, Me, "PR"))
                Next
            Else
                TI.Add(0)
                PI.Add(0)
            End If

            If TVB.Count > 1 Then TVB.RemoveAt(TVB.Count - 1)
            If PB.Count > 1 Then PB.RemoveAt(PB.Count - 1)
            If HB.Count > 1 Then HB.RemoveAt(HB.Count - 1)
            If SB.Count > 1 Then SB.RemoveAt(SB.Count - 1)
            If VB.Count > 1 Then VB.RemoveAt(VB.Count - 1)
            If TVB.Count > 1 Then TVB.RemoveAt(TVB.Count - 1)
            If PB.Count > 1 Then PB.RemoveAt(PB.Count - 1)
            If HB.Count > 1 Then HB.RemoveAt(HB.Count - 1)
            If SB.Count > 1 Then SB.RemoveAt(SB.Count - 1)
            If VB.Count > 1 Then VB.RemoveAt(VB.Count - 1)

            If TOWF.Count > 1 Then TOWF.RemoveAt(TOWF.Count - 1)
            If POWF.Count > 1 Then POWF.RemoveAt(POWF.Count - 1)
            If HOWF.Count > 1 Then HOWF.RemoveAt(HOWF.Count - 1)
            If SOWF.Count > 1 Then SOWF.RemoveAt(SOWF.Count - 1)
            If VOWF.Count > 1 Then VOWF.RemoveAt(VOWF.Count - 1)

            If TVD.Count > 1 Then TVD.RemoveAt(TVD.Count - 1)
            If PO.Count > 1 Then PO.RemoveAt(PO.Count - 1)
            If HO.Count > 1 Then HO.RemoveAt(HO.Count - 1)
            If SO.Count > 1 Then SO.RemoveAt(SO.Count - 1)
            If VO.Count > 1 Then VO.RemoveAt(VO.Count - 1)

            If TVD.Count > 1 Then TVD.RemoveAt(TVD.Count - 1)
            If PO.Count > 1 Then PO.RemoveAt(PO.Count - 1)
            If HO.Count > 1 Then HO.RemoveAt(HO.Count - 1)
            If SO.Count > 1 Then SO.RemoveAt(SO.Count - 1)
            If VO.Count > 1 Then VO.RemoveAt(VO.Count - 1)

            If TQ.Count > 1 Then TQ.RemoveAt(TQ.Count - 1)
            If PQ.Count > 1 Then PQ.RemoveAt(PQ.Count - 1)
            If TI.Count > 1 Then TI.RemoveAt(TI.Count - 1)
            If PI.Count > 1 Then PI.RemoveAt(PI.Count - 1)

            Return New Object() {TVB, PB, HB, SB, VB, TVD, PO, HO, SO, VO, TE, PE, TH, PHsI, PHsII, CP, TQ, PQ, TI, PI, TOWF, POWF, HOWF, SOWF, VOWF}

        End Function

        Public Function DW_ReturnPhaseEnvelopeParallel(ByVal parameters As Object, Optional ByVal bw As System.ComponentModel.BackgroundWorker = Nothing) As Object

            Dim cpc As New DWSIM.Utilities.TCP.Methods
            Dim i, j, k, l As Integer
            Dim n As Integer = Me.CurrentMaterialStream.Fases(0).Componentes.Count - 1
            Dim Vz(n) As Double
            Dim comp As DWSIM.ClassesBasicasTermodinamica.Substancia
            i = 0
            For Each comp In Me.CurrentMaterialStream.Fases(0).Componentes.Values
                Vz(i) += comp.FracaoMolar.GetValueOrDefault
                i += 1
            Next
            i = 0
            Do
                If Vz(i) = 0 Then j += 1
                i = i + 1
            Loop Until i = n + 1
            Dim VTc(n), Vpc(n), Vw(n), VVc(n), VKij(n, n) As Double
            Dim Vm2(UBound(Vz) - j), VPc2(UBound(Vz) - j), VTc2(UBound(Vz) - j), VVc2(UBound(Vz) - j), Vw2(UBound(Vz) - j), VKij2(UBound(Vz) - j, UBound(Vz) - j)
            VTc = Me.RET_VTC()
            Vpc = Me.RET_VPC()
            VVc = Me.RET_VVC()
            Vw = Me.RET_VW()
            VKij = Me.RET_VKij
            i = 0
            k = 0
            Do
                If Vz(i) <> 0 Then
                    Vm2(k) = Vz(i)
                    VTc2(k) = VTc(i)
                    VPc2(k) = Vpc(i)
                    VVc2(k) = VVc(i)
                    Vw2(k) = Vw(i)
                    j = 0
                    l = 0
                    Do
                        If Vz(l) <> 0 Then
                            VKij2(k, j) = VKij(i, l)
                            j = j + 1
                        End If
                        l = l + 1
                    Loop Until l = n + 1
                    k = k + 1
                End If
                i = i + 1
            Loop Until i = n + 1
            Dim PB, PO, TVB, TVD, HB, HO, SB, SO, VB, VO, TE, PE, TH, PHsI, PHsII, TQ, PQ, TI, PI, TOWF, POWF, VOWF, HOWF, SOWF As New ArrayList
            Dim TCR, PCR, VCR As Double
            Dim CP As New ArrayList

            My.MyApplication.IsRunningParallelTasks = True

            Dim tasks(6) As task

            tasks(0) = Task.Factory.StartNew(Sub()
                                                 If n > 0 Then
                                                     CP = cpc.CRITPT_PR(Vm2, VTc2, VPc2, VVc2, Vw2, VKij2)
                                                     If CP.Count > 0 Then
                                                         Dim cp0 = CP(0)
                                                         TCR = cp0(0)
                                                         PCR = cp0(1)
                                                         VCR = cp0(2)
                                                     Else
                                                         TCR = 0
                                                         PCR = 0
                                                         VCR = 0
                                                     End If
                                                 Else
                                                     TCR = Me.AUX_TCM(Fase.Mixture)
                                                     PCR = Me.AUX_PCM(Fase.Mixture)
                                                     VCR = Me.AUX_VCM(Fase.Mixture)
                                                     CP.Add(New Object() {TCR, PCR, VCR})
                                                 End If
                                             End Sub)



            Dim beta As Double = 10

            Task.WaitAll(tasks(0))

            tasks(1) = Task.Factory.StartNew(Sub()
                                                 Dim Pmin, Tmin, dP, dT, T, P As Double
                                                 Pmin = 101325
                                                 Tmin = 0.3 * TCR
                                                 dP = (PCR - Pmin) / 50
                                                 dT = (TCR - Tmin) / 50
                                                 Dim tmp2 As Object
                                                 Dim KI(n) As Double
                                                 j = 0
                                                 Do
                                                     KI(j) = 0
                                                     j = j + 1
                                                 Loop Until j = n + 1
                                                 Dim ii As Integer = 0
                                                 P = Pmin
                                                 T = Tmin
                                                 Do
                                                     If ii < 2 Then
                                                         tmp2 = Me.FlashBase.Flash_PV(Vz, P, 0, 0, Me)
                                                         TVB.Add(tmp2(4))
                                                         PB.Add(P)
                                                         T = TVB(ii)
                                                         HB.Add(Me.DW_CalcEnthalpy(Vz, T, P, State.Liquid))
                                                         SB.Add(Me.DW_CalcEntropy(Vz, T, P, State.Liquid))
                                                         VB.Add(Me.m_pr.Z_PR(T, P, Vz, Me.RET_VKij, Me.RET_VTC, Me.RET_VPC, Me.RET_VW, "L") * 8.314 * T / P)
                                                         P = P + dP
                                                         KI = tmp2(6)
                                                     Else
                                                         If beta < 20 Then
                                                             tmp2 = Me.FlashBase.Flash_TV(Vz, T, 0, PB(ii - 1), Me, True, KI)
                                                             'tmp2 = BUBP_PR_M2(T, Vz, Me.RET_VKij, Me.RET_VTC, Me.RET_VPC, Me.RET_VTB, Me.RET_VW, KI, PB(ii - 1))
                                                             TVB.Add(T)
                                                             PB.Add(tmp2(4))
                                                             P = PB(ii)
                                                             HB.Add(Me.DW_CalcEnthalpy(Vz, T, P, State.Liquid))
                                                             SB.Add(Me.DW_CalcEntropy(Vz, T, P, State.Liquid))
                                                             VB.Add(Me.m_pr.Z_PR(T, P, Vz, Me.RET_VKij, Me.RET_VTC, Me.RET_VPC, Me.RET_VW, "L") * 8.314 * T / P)
                                                             If Math.Abs(T - TCR) / TCR < 0.01 And Math.Abs(P - PCR) / PCR < 0.02 Then
                                                                 T = T + dT * 0.5
                                                             Else
                                                                 T = T + dT
                                                             End If
                                                             KI = tmp2(6)
                                                         Else
                                                             tmp2 = Me.FlashBase.Flash_PV(Vz, P, 0, TVB(ii - 1), Me, True, KI)
                                                             TVB.Add(tmp2(4))
                                                             PB.Add(P)
                                                             T = TVB(ii)
                                                             HB.Add(Me.DW_CalcEnthalpy(Vz, T, P, State.Liquid))
                                                             SB.Add(Me.DW_CalcEntropy(Vz, T, P, State.Liquid))
                                                             VB.Add(Me.m_pr.Z_PR(T, P, Vz, Me.RET_VKij, Me.RET_VTC, Me.RET_VPC, Me.RET_VW, "L") * 8.314 * T / P)
                                                             If Math.Abs(T - TCR) / TCR < 0.01 And Math.Abs(P - PCR) / PCR < 0.01 Then
                                                                 P = P + dP * 0.1
                                                             Else
                                                                 P = P + dP
                                                             End If
                                                             KI = tmp2(6)
                                                         End If
                                                         beta = (Math.Log(PB(ii) / 101325) - Math.Log(PB(ii - 1) / 101325)) / (Math.Log(TVB(ii)) - Math.Log(TVB(ii - 1)))
                                                     End If
                                                     ii = ii + 1
                                                 Loop Until ii >= 200 Or PB(ii - 1) = 0 Or PB(ii - 1) < 0 Or TVB(ii - 1) < 0 Or _
                                                             T >= TCR Or Double.IsNaN(PB(ii - 1)) = True Or _
                                                             Double.IsNaN(TVB(ii - 1)) = True Or Math.Abs(T - TCR) / TCR < 0.002 And _
                                                             Math.Abs(P - PCR) / PCR < 0.002
                                             End Sub)

            tasks(2) = Task.Factory.StartNew(Sub()
                                                 Dim Switch = False
                                                 beta = 10
                                                 Dim Pmin, Tmin, dP, dT, T, P As Double
                                                 Pmin = 101325
                                                 Tmin = 0.3 * TCR
                                                 dP = (PCR - Pmin) / 50
                                                 dT = (TCR - Tmin) / 50
                                                 Dim tmp2 As Object
                                                 Dim KI(n) As Double
                                                 j = 0
                                                 Do
                                                     KI(j) = 0
                                                     j = j + 1
                                                 Loop Until j = n + 1
                                                 Dim ii As Integer = 0
                                                 P = Pmin
                                                 Do
                                                     If ii < 2 Then
                                                         tmp2 = Me.FlashBase.Flash_PV(Vz, P, 1, 0, Me)
                                                         TVD.Add(tmp2(4))
                                                         PO.Add(P)
                                                         T = TVD(ii)
                                                         HO.Add(Me.DW_CalcEnthalpy(Vz, T, P, State.Vapor))
                                                         SO.Add(Me.DW_CalcEntropy(Vz, T, P, State.Vapor))
                                                         VO.Add(Me.m_pr.Z_PR(T, P, Vz, Me.RET_VKij, Me.RET_VTC, Me.RET_VPC, Me.RET_VW, "V") * 8.314 * T / P)
                                                         If Math.Abs(T - TCR) / TCR < 0.01 And Math.Abs(P - PCR) / PCR < 0.01 Then
                                                             P = P + dP * 0.1
                                                         Else
                                                             P = P + dP
                                                         End If
                                                         KI = tmp2(6)
                                                     Else
                                                         If Abs(beta) < 2 Then
                                                             tmp2 = Me.FlashBase.Flash_TV(Vz, T, 1, PO(ii - 1), Me, True, KI)
                                                             TVD.Add(T)
                                                             PO.Add(tmp2(4))
                                                             P = PO(ii)
                                                             HO.Add(Me.DW_CalcEnthalpy(Vz, T, P, State.Vapor))
                                                             SO.Add(Me.DW_CalcEntropy(Vz, T, P, State.Vapor))
                                                             VO.Add(Me.m_pr.Z_PR(T, P, Vz, Me.RET_VKij, Me.RET_VTC, Me.RET_VPC, Me.RET_VW, "V") * 8.314 * T / P)
                                                             If TVD(ii) - TVD(ii - 1) <= 0 Then
                                                                 If Math.Abs(T - TCR) / TCR < 0.02 And Math.Abs(P - PCR) / PCR < 0.02 Then
                                                                     T = T - dT * 0.1
                                                                 Else
                                                                     T = T - dT
                                                                 End If
                                                             Else
                                                                 If Math.Abs(T - TCR) / TCR < 0.02 And Math.Abs(P - PCR) / PCR < 0.02 Then
                                                                     T = T + dT * 0.1
                                                                 Else
                                                                     T = T + dT
                                                                 End If
                                                             End If
                                                             KI = tmp2(6)
                                                         Else
                                                             tmp2 = Me.FlashBase.Flash_PV(Vz, P, 1, TVD(ii - 1), Me, False, KI)
                                                             TVD.Add(tmp2(4))
                                                             PO.Add(P)
                                                             T = TVD(ii)
                                                             HO.Add(Me.DW_CalcEnthalpy(Vz, T, P, State.Vapor))
                                                             SO.Add(Me.DW_CalcEntropy(Vz, T, P, State.Vapor))
                                                             VO.Add(Me.m_pr.Z_PR(T, P, Vz, Me.RET_VKij, Me.RET_VTC, Me.RET_VPC, Me.RET_VW, "V") * 8.314 * T / P)
                                                             If Math.Abs(T - TCR) / TCR < 0.05 And Math.Abs(P - PCR) / PCR < 0.05 Then
                                                                 P = P + dP * 0.25
                                                             Else
                                                                 P = P + dP
                                                             End If
                                                             KI = tmp2(6)
                                                         End If
                                                         If ii >= PO.Count Then
                                                             ii = ii - 1
                                                         End If
                                                         beta = (Math.Log(PO(ii) / 101325) - Math.Log(PO(ii - 1) / 101325)) / (Math.Log(TVD(ii)) - Math.Log(TVD(ii - 1)))
                                                         If Double.IsNaN(beta) Or Double.IsInfinity(beta) Then beta = 0
                                                     End If
                                                     ii = ii + 1
                                                 Loop Until ii >= 200 Or PO(ii - 1) = 0 Or PO(ii - 1) < 0 Or TVD(ii - 1) < 0 Or _
                                                             Double.IsNaN(PO(ii - 1)) = True Or Double.IsNaN(TVD(ii - 1)) = True Or _
                                                             (Math.Abs(T - TCR) / TCR < 0.03 And Math.Abs(P - PCR) / PCR < 0.01)

                                             End Sub)

            tasks(6) = Task.Factory.StartNew(Sub()

                                                 If Me.RET_VCAS().Contains("7732-18-5") Then

                                                     Dim ii As Integer = 0
                                                     Dim wi As Integer = Array.IndexOf(Me.RET_VCAS(), "7732-18-5")
                                                     Dim wmf As Double = Vz(wi)
                                                     Dim Vzwf(n) As Double
                                                     For ii = 0 To n
                                                         If ii <> wi Then
                                                             Vzwf(ii) = Vz(ii) / (1 - wmf)
                                                         Else
                                                             Vzwf(ii) = 0.0#
                                                         End If
                                                     Next

                                                     beta = 10
                                                     Dim Pmin, Tmin, dP, dT, T, P As Double
                                                     Pmin = 101325
                                                     Tmin = 0.3 * TCR
                                                     dP = (PCR - Pmin) / 50
                                                     dT = (TCR - Tmin) / 50
                                                     Dim tmp2 As Object
                                                     Dim KI(n) As Double
                                                     j = 0
                                                     Do
                                                         KI(j) = 0
                                                         j = j + 1
                                                     Loop Until j = n + 1
                                                     P = Pmin

                                                     'water-free calc
                                                     ii = 0
                                                     P = Pmin
                                                     Do
                                                         If ii < 2 Then
                                                             Try
                                                                 tmp2 = Me.FlashBase.Flash_PV(Vzwf, P, 1, 0, Me)
                                                                 TOWF.Add(tmp2(4))
                                                                 POWF.Add(P)
                                                                 T = TOWF(ii)
                                                                 HOWF.Add(Me.DW_CalcEnthalpy(Vzwf, T, P, State.Vapor))
                                                                 SOWF.Add(Me.DW_CalcEntropy(Vzwf, T, P, State.Vapor))
                                                                 VOWF.Add(1 / Me.AUX_VAPDENS(T, P) * Me.AUX_MMM(Vzwf))
                                                                 KI = tmp2(6)
                                                             Catch ex As Exception
                                                             Finally
                                                                 If Math.Abs(T - TCR) / TCR < 0.01 And Math.Abs(P - PCR) / PCR < 0.01 Then
                                                                     P = P + dP * 0.1
                                                                 Else
                                                                     P = P + dP
                                                                 End If
                                                             End Try
                                                         Else
                                                             If Abs(beta) < 2 Then
                                                                 Try
                                                                     tmp2 = Me.FlashBase.Flash_TV(Vzwf, T, 1, POWF(ii - 1), Me, True, KI)
                                                                     TOWF.Add(T)
                                                                     POWF.Add(tmp2(4))
                                                                     P = POWF(ii)
                                                                     HOWF.Add(Me.DW_CalcEnthalpy(Vzwf, T, P, State.Vapor))
                                                                     SOWF.Add(Me.DW_CalcEntropy(Vzwf, T, P, State.Vapor))
                                                                     VOWF.Add(1 / Me.AUX_VAPDENS(T, P) * Me.AUX_MMM(Vzwf))
                                                                     KI = tmp2(6)
                                                                 Catch ex As Exception
                                                                 Finally
                                                                     If TOWF(ii) - TOWF(ii - 1) <= 0 Then
                                                                         If Math.Abs(T - TCR) / TCR < 0.02 And Math.Abs(P - PCR) / PCR < 0.02 Then
                                                                             T = T - dT * 0.1
                                                                         Else
                                                                             T = T - dT
                                                                         End If
                                                                     Else
                                                                         If Math.Abs(T - TCR) / TCR < 0.02 And Math.Abs(P - PCR) / PCR < 0.02 Then
                                                                             T = T + dT * 0.1
                                                                         Else
                                                                             T = T + dT
                                                                         End If
                                                                     End If
                                                                 End Try
                                                             Else
                                                                 Try
                                                                     tmp2 = Me.FlashBase.Flash_PV(Vzwf, P, 1, TOWF(ii - 1), Me, False, KI)
                                                                     TOWF.Add(tmp2(4))
                                                                     POWF.Add(P)
                                                                     T = TOWF(ii)
                                                                     HOWF.Add(Me.DW_CalcEnthalpy(Vzwf, T, P, State.Vapor))
                                                                     SOWF.Add(Me.DW_CalcEntropy(Vzwf, T, P, State.Vapor))
                                                                     VOWF.Add(1 / Me.AUX_VAPDENS(T, P) * Me.AUX_MMM(Vzwf))
                                                                     KI = tmp2(6)
                                                                 Catch ex As Exception
                                                                 Finally
                                                                     If Math.Abs(T - TCR) / TCR < 0.05 And Math.Abs(P - PCR) / PCR < 0.05 Then
                                                                         P = P + dP * 0.25
                                                                     Else
                                                                         P = P + dP
                                                                     End If
                                                                 End Try
                                                             End If
                                                             If ii >= POWF.Count Then
                                                                 ii = ii - 1
                                                             End If
                                                             beta = (Math.Log(POWF(ii) / 101325) - Math.Log(POWF(ii - 1) / 101325)) / (Math.Log(TOWF(ii)) - Math.Log(TOWF(ii - 1)))
                                                             If Double.IsNaN(beta) Or Double.IsInfinity(beta) Then beta = 0
                                                         End If
                                                         ii = ii + 1
                                                     Loop Until ii >= 200 Or POWF(ii - 1) = 0 Or POWF(ii - 1) < 0 Or TOWF(ii - 1) < 0 Or _
                                                                 Double.IsNaN(POWF(ii - 1)) = True Or Double.IsNaN(TOWF(ii - 1)) = True Or _
                                                                 Math.Abs(T - TCR) / TCR < 0.03 And Math.Abs(P - PCR) / PCR < 0.03

                                                 End If

                                             End Sub)

            tasks(3) = Task.Factory.StartNew(Sub()

                                                 If CBool(parameters(2)) = True Then
                                                     beta = 10
                                                     Dim Pmin, Tmin, dP, dT, T, P As Double
                                                     Pmin = 101325
                                                     Tmin = 0.3 * TCR
                                                     dP = (PCR - Pmin) / 50
                                                     dT = (TCR - Tmin) / 50
                                                     Dim tmp2 As Object
                                                     Dim KI(n) As Double
                                                     j = 0
                                                     Do
                                                         KI(j) = 0
                                                         j = j + 1
                                                     Loop Until j = n + 1
                                                     Dim ii As Integer = 0
                                                     P = 101325
                                                     T = 0.3 * TCR
                                                     Do
                                                         If ii < 2 Then
                                                             tmp2 = Me.FlashBase.Flash_PV(Vz, P, parameters(1), 0, Me, False, KI)
                                                             TQ.Add(tmp2(4))
                                                             PQ.Add(P)
                                                             T = TQ(ii)
                                                             P = P + dP
                                                             KI = tmp2(6)
                                                         Else
                                                             If beta < 2 Then
                                                                 tmp2 = Me.FlashBase.Flash_TV(Vz, T, parameters(1), PQ(ii - 1), Me, True, KI)
                                                                 TQ.Add(T)
                                                                 PQ.Add(tmp2(4))
                                                                 P = PQ(ii)
                                                                 If Math.Abs(T - TCR) / TCR < 0.1 And Math.Abs(P - PCR) / PCR < 0.2 Then
                                                                     T = T + dT * 0.25
                                                                 Else
                                                                     T = T + dT
                                                                 End If
                                                                 KI = tmp2(6)
                                                             Else
                                                                 tmp2 = Me.FlashBase.Flash_PV(Vz, P, parameters(1), TQ(ii - 1), Me, True, KI)
                                                                 TQ.Add(tmp2(4))
                                                                 PQ.Add(P)
                                                                 T = TQ(ii)
                                                                 If Math.Abs(T - TCR) / TCR < 0.1 And Math.Abs(P - PCR) / PCR < 0.1 Then
                                                                     P = P + dP * 0.1
                                                                 Else
                                                                     P = P + dP
                                                                 End If
                                                                 KI = tmp2(6)
                                                             End If
                                                             beta = (Math.Log(PQ(ii) / 101325) - Math.Log(PQ(ii - 1) / 101325)) / (Math.Log(TQ(ii)) - Math.Log(TQ(ii - 1)))
                                                         End If
                                                         ii = ii + 1
                                                         If ii > 2 Then
                                                             If PQ(ii - 1) = PQ(ii - 2) Or TQ(ii - 1) = TQ(ii - 2) Then Exit Do
                                                         End If
                                                     Loop Until ii >= 200 Or PQ(ii - 1) = 0 Or PQ(ii - 1) < 0 Or TQ(ii - 1) < 0 Or _
                                                                 Double.IsNaN(PQ(ii - 1)) = True Or Double.IsNaN(TQ(ii - 1)) = True Or _
                                                                 Math.Abs(T - TCR) / TCR < 0.02 And Math.Abs(P - PCR) / PCR < 0.02

                                                 Else
                                                     TQ.Add(0)
                                                     PQ.Add(0)
                                                 End If
                                             End Sub)


            tasks(4) = Task.Factory.StartNew(Sub()
                                                 If n > 0 And CBool(parameters(3)) = True Then
                                                     Dim res As ArrayList = cpc.STABILITY_CURVE(Vm2, VTc2, VPc2, VVc2, Vw2, VKij2)
                                                     i = 0
                                                     Do
                                                         TE.Add(res(i)(0))
                                                         PE.Add(res(i)(1))
                                                         i += 1
                                                     Loop Until i = res.Count
                                                 Else
                                                     TE.Add(0)
                                                     PE.Add(0)
                                                 End If
                                             End Sub)

            tasks(5) = Task.Factory.StartNew(Sub()
                                                 Dim Pest, Tmax, Tmin, T As Double
                                                 Pest = PCR * 10
                                                 Tmin = MathEx.Common.Max(Me.RET_VTF)
                                                 If Tmin = 0.0# Then Tmin = MathEx.Common.Min(Me.RET_VTB) * 0.4
                                                 Tmax = TCR * 1.4
                                                 If CBool(parameters(4)) = True Then
                                                     For T = Tmin To Tmax Step 5
                                                         TI.Add(T)
                                                         PI.Add(Auxiliary.FlashAlgorithms.FlashAlgorithm.CalcPIPressure(Vz, Pest, T, Me, "PR"))
                                                     Next
                                                 Else
                                                     TI.Add(0)
                                                     PI.Add(0)
                                                 End If
                                             End Sub)

            Try
                Task.WaitAll(New Task() {tasks(1), tasks(2), tasks(3), tasks(4), tasks(5), tasks(6)})
            Catch ae As AggregateException
                Throw ae.Flatten().InnerException
            End Try

            If TVB.Count > 1 Then TVB.RemoveAt(TVB.Count - 1)
            If PB.Count > 1 Then PB.RemoveAt(PB.Count - 1)
            If HB.Count > 1 Then HB.RemoveAt(HB.Count - 1)
            If SB.Count > 1 Then SB.RemoveAt(SB.Count - 1)
            If VB.Count > 1 Then VB.RemoveAt(VB.Count - 1)
            If TVB.Count > 1 Then TVB.RemoveAt(TVB.Count - 1)
            If PB.Count > 1 Then PB.RemoveAt(PB.Count - 1)
            If HB.Count > 1 Then HB.RemoveAt(HB.Count - 1)
            If SB.Count > 1 Then SB.RemoveAt(SB.Count - 1)
            If VB.Count > 1 Then VB.RemoveAt(VB.Count - 1)

            If ToWF.Count > 1 Then ToWF.RemoveAt(ToWF.Count - 1)
            If PoWF.Count > 1 Then PoWF.RemoveAt(PoWF.Count - 1)
            If HoWF.Count > 1 Then HoWF.RemoveAt(HoWF.Count - 1)
            If SoWF.Count > 1 Then SoWF.RemoveAt(SoWF.Count - 1)
            If VoWF.Count > 1 Then VoWF.RemoveAt(VoWF.Count - 1)

            If TVD.Count > 1 Then TVD.RemoveAt(TVD.Count - 1)
            If PO.Count > 1 Then PO.RemoveAt(PO.Count - 1)
            If HO.Count > 1 Then HO.RemoveAt(HO.Count - 1)
            If SO.Count > 1 Then SO.RemoveAt(SO.Count - 1)
            If VO.Count > 1 Then VO.RemoveAt(VO.Count - 1)

            If TVD.Count > 1 Then TVD.RemoveAt(TVD.Count - 1)
            If PO.Count > 1 Then PO.RemoveAt(PO.Count - 1)
            If HO.Count > 1 Then HO.RemoveAt(HO.Count - 1)
            If SO.Count > 1 Then SO.RemoveAt(SO.Count - 1)
            If VO.Count > 1 Then VO.RemoveAt(VO.Count - 1)

            If TQ.Count > 1 Then TQ.RemoveAt(TQ.Count - 1)
            If PQ.Count > 1 Then PQ.RemoveAt(PQ.Count - 1)
            If TI.Count > 1 Then TI.RemoveAt(TI.Count - 1)
            If PI.Count > 1 Then PI.RemoveAt(PI.Count - 1)

            Return New Object() {TVB, PB, HB, SB, VB, TVD, PO, HO, SO, VO, TE, PE, TH, PHsI, PHsII, CP, TQ, PQ, TI, PI, ToWF, PoWF, HoWF, SoWF, VoWF}

        End Function

        Public Function RET_KIJ(ByVal id1 As String, ByVal id2 As String) As Double
            If Me.m_pr.InteractionParameters.ContainsKey(id1) Then
                If Me.m_pr.InteractionParameters(id1).ContainsKey(id2) Then
                    Return m_pr.InteractionParameters(id1)(id2).kij
                Else
                    If Me.m_pr.InteractionParameters.ContainsKey(id2) Then
                        If Me.m_pr.InteractionParameters(id2).ContainsKey(id1) Then
                            Return m_pr.InteractionParameters(id2)(id1).kij
                        Else
                            Return 0
                        End If
                    Else
                        Return 0
                    End If
                End If
            Else
                Return 0
            End If
        End Function

        Public Overrides Function RET_VKij() As Double(,)

            Dim val(Me.CurrentMaterialStream.Fases(0).Componentes.Count - 1, Me.CurrentMaterialStream.Fases(0).Componentes.Count - 1) As Double
            Dim i As Integer = 0
            Dim l As Integer = 0

            i = 0
            For Each cp As ClassesBasicasTermodinamica.Substancia In Me.CurrentMaterialStream.Fases(0).Componentes.Values
                l = 0
                For Each cp2 As ClassesBasicasTermodinamica.Substancia In Me.CurrentMaterialStream.Fases(0).Componentes.Values
                    val(i, l) = Me.RET_KIJ(cp.Nome, cp2.Nome)
                    l = l + 1
                Next
                i = i + 1
            Next

            Return val

        End Function

        Public Function AUX_CM(ByVal Vx As Object) As Double

            Dim val As Double
            Dim subst As DWSIM.ClassesBasicasTermodinamica.Substancia

            Dim i As Integer = 0
            For Each subst In Me.CurrentMaterialStream.Fases(0).Componentes.Values
                val += Vx(i) * subst.ConstantProperties.PR_Volume_Translation_Coefficient * Me.m_pr.bi(0.0778, subst.ConstantProperties.Critical_Temperature, subst.ConstantProperties.Critical_Pressure)
                i += 1
            Next

            Return val

        End Function

        Public Function AUX_CM(ByVal fase As Fase) As Double

            Dim val As Double
            Dim subst As DWSIM.ClassesBasicasTermodinamica.Substancia

            For Each subst In Me.CurrentMaterialStream.Fases(Me.RET_PHASEID(fase)).Componentes.Values
                val += subst.FracaoMolar.GetValueOrDefault * subst.ConstantProperties.PR_Volume_Translation_Coefficient * Me.m_pr.bi(0.0778, subst.ConstantProperties.Critical_Temperature, subst.ConstantProperties.Critical_Pressure)
            Next

            Return val

        End Function

        Public Function RET_VS() As Double()

            Dim val(Me.CurrentMaterialStream.Fases(0).Componentes.Count - 1) As Double
            Dim subst As DWSIM.ClassesBasicasTermodinamica.Substancia
            Dim i As Integer = 0

            For Each subst In Me.CurrentMaterialStream.Fases(0).Componentes.Values
                val(i) = subst.ConstantProperties.PR_Volume_Translation_Coefficient
                i += 1
            Next

            Return val

        End Function

        Public Function RET_VC() As Double()

            Dim val(Me.CurrentMaterialStream.Fases(0).Componentes.Count - 1) As Double
            Dim subst As DWSIM.ClassesBasicasTermodinamica.Substancia
            Dim i As Integer = 0

            For Each subst In Me.CurrentMaterialStream.Fases(0).Componentes.Values
                val(i) = subst.ConstantProperties.PR_Volume_Translation_Coefficient * Me.m_pr.bi(0.0778, subst.ConstantProperties.Critical_Temperature, subst.ConstantProperties.Critical_Pressure)
                i += 1
            Next

            Return val

        End Function

#End Region

#Region "    Métodos Numéricos"

        Public Function IntegralSimpsonCp(ByVal a As Double, _
                 ByVal b As Double, _
                 ByVal Epsilon As Double, ByVal subst As String) As Double

            Dim Result As Double
            Dim switch As Boolean = False
            Dim h As Double
            Dim s As Double
            Dim s1 As Double
            Dim s2 As Double
            Dim s3 As Double
            Dim x As Double
            Dim tm As Double

            If a > b Then
                switch = True
                tm = a
                a = b
                b = tm
            ElseIf Abs(a - b) < 0.01 Then
                Return 0
            End If

            s2 = 1.0#
            h = b - a
            s = Me.AUX_CPi(subst, a) + Me.AUX_CPi(subst, b)
            Do
                s3 = s2
                h = h / 2.0#
                s1 = 0.0#
                x = a + h
                Do
                    s1 = s1 + 2.0# * Me.AUX_CPi(subst, x)
                    x = x + 2.0# * h
                Loop Until Not x < b
                s = s + s1
                s2 = (s + s1) * h / 3.0#
                x = Abs(s3 - s2) / 15.0#
            Loop Until Not x > Epsilon
            Result = s2

            If switch Then Result = -Result

            IntegralSimpsonCp = Result

        End Function

        Public Function IntegralSimpsonCp_T(ByVal a As Double, _
         ByVal b As Double, _
         ByVal Epsilon As Double, ByVal subst As String) As Double

            'Cp = A + B*T + C*T^2 + D*T^3 + E*T^4 where Cp in kJ/kg-mol , T in K 


            Dim Result As Double
            Dim h As Double
            Dim s As Double
            Dim s1 As Double
            Dim s2 As Double
            Dim s3 As Double
            Dim x As Double
            Dim tm As Double
            Dim switch As Boolean = False

            If a > b Then
                switch = True
                tm = a
                a = b
                b = tm
            ElseIf Abs(a - b) < 0.01 Then
                Return 0
            End If

            s2 = 1.0#
            h = b - a
            s = Me.AUX_CPi(subst, a) / a + Me.AUX_CPi(subst, b) / b
            Do
                s3 = s2
                h = h / 2.0#
                s1 = 0.0#
                x = a + h
                Do
                    s1 = s1 + 2.0# * Me.AUX_CPi(subst, x) / x
                    x = x + 2.0# * h
                Loop Until Not x < b
                s = s + s1
                s2 = (s + s1) * h / 3.0#
                x = Abs(s3 - s2) / 15.0#
            Loop Until Not x > Epsilon
            Result = s2

            If switch Then Result = -Result

            IntegralSimpsonCp_T = Result

        End Function

#End Region

        Public Overrides Function DW_CalcEnthalpy(ByVal Vx As System.Array, ByVal T As Double, ByVal P As Double, ByVal st As State) As Double

            Dim H As Double

            If st = State.Liquid Then
                H = Me.m_pr.H_PR_MIX("L", T, P, Vx, RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, Me.RET_Hid(298.15, T, Vx))
            Else
                H = Me.m_pr.H_PR_MIX("V", T, P, Vx, RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, Me.RET_Hid(298.15, T, Vx))
            End If

            Return H

        End Function

        Public Overrides Function DW_CalcEnthalpyDeparture(ByVal Vx As System.Array, ByVal T As Double, ByVal P As Double, ByVal st As State) As Double

            Dim H As Double

            If st = State.Liquid Then
                H = Me.m_pr.H_PR_MIX("L", T, P, Vx, RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, 0)
            Else
                H = Me.m_pr.H_PR_MIX("V", T, P, Vx, RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, 0)
            End If

            Return H

        End Function

        Public Overrides Function DW_CalcEntropy(ByVal Vx As System.Array, ByVal T As Double, ByVal P As Double, ByVal st As State) As Double

            Dim S As Double

            If st = State.Liquid Then
                S = Me.m_pr.S_PR_MIX("L", T, P, Vx, RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, Me.RET_Sid(298.15, T, P, Vx))
            Else
                S = Me.m_pr.S_PR_MIX("V", T, P, Vx, RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, Me.RET_Sid(298.15, T, P, Vx))
            End If

            Return S

        End Function

        Public Overrides Function DW_CalcEntropyDeparture(ByVal Vx As System.Array, ByVal T As Double, ByVal P As Double, ByVal st As State) As Double
            Dim S As Double

            If st = State.Liquid Then
                S = Me.m_pr.S_PR_MIX("L", T, P, Vx, RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, 0)
            Else
                S = Me.m_pr.S_PR_MIX("V", T, P, Vx, RET_VKij(), RET_VTC, RET_VPC, RET_VW, RET_VMM, 0)
            End If

            Return S

        End Function

        Public Overrides Function DW_CalcCv_ISOL(ByVal fase1 As Fase, ByVal T As Double, ByVal P As Double) As Double
            Select Case fase1
                Case Fase.Liquid
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(fase1), RET_VKij(), RET_VMAS(fase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(2)
                Case Fase.Aqueous
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(fase1), RET_VKij(), RET_VMAS(fase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(2)
                Case Fase.Liquid1
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(fase1), RET_VKij(), RET_VMAS(fase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(2)
                Case Fase.Liquid2
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(fase1), RET_VKij(), RET_VMAS(fase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(2)
                Case Fase.Liquid3
                    Return Me.m_props.CpCvR("L", T, P, RET_VMOL(fase1), RET_VKij(), RET_VMAS(fase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(2)
                Case Fase.Vapor
                    Return Me.m_props.CpCvR("V", T, P, RET_VMOL(fase1), RET_VKij(), RET_VMAS(fase1), RET_VTC(), RET_VPC(), RET_VCP(T), RET_VMM(), RET_VW(), RET_VZRa())(2)
            End Select
        End Function

        Public Overrides Sub DW_CalcCompPartialVolume(ByVal phase As Fase, ByVal T As Double, ByVal P As Double)

            Dim partvol As New Object
            Dim key As String = "0"
            Dim i As Integer = 0

            If Not Me.Parameters.ContainsKey("PP_USE_EOS_LIQDENS") Then Me.Parameters.Add("PP_USE_EOS_LIQDENS", 0)

            Select Case phase
                Case Fase.Liquid
                    key = "1"
                    If CInt(Me.Parameters("PP_USE_EOS_LIQDENS")) = 1 Then
                        partvol = Me.m_pr.CalcPartialVolume(T, P, RET_VMOL(phase), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VTB(), "L", 0.01)
                    Else
                        partvol = New ArrayList
                        For Each subst As ClassesBasicasTermodinamica.Substancia In Me.CurrentMaterialStream.Fases(key).Componentes.Values
                            partvol.Add(1 / 1000 * subst.ConstantProperties.Molar_Weight / Me.m_props.liq_dens_rackett(T, subst.ConstantProperties.Critical_Temperature, subst.ConstantProperties.Critical_Pressure, subst.ConstantProperties.Acentric_Factor, subst.ConstantProperties.Molar_Weight, subst.ConstantProperties.Z_Rackett, P, Me.AUX_PVAPi(subst.Nome, T)))
                        Next
                    End If
                Case Fase.Aqueous
                    key = "6"
                    If CInt(Me.Parameters("PP_USE_EOS_LIQDENS")) = 1 Then
                        partvol = Me.m_pr.CalcPartialVolume(T, P, RET_VMOL(phase), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VTB(), "L", 0.01)
                    Else
                        partvol = New ArrayList
                        For Each subst As ClassesBasicasTermodinamica.Substancia In Me.CurrentMaterialStream.Fases(key).Componentes.Values
                            partvol.Add(1 / 1000 * subst.ConstantProperties.Molar_Weight / Me.m_props.liq_dens_rackett(T, subst.ConstantProperties.Critical_Temperature, subst.ConstantProperties.Critical_Pressure, subst.ConstantProperties.Acentric_Factor, subst.ConstantProperties.Molar_Weight, subst.ConstantProperties.Z_Rackett, P, Me.AUX_PVAPi(subst.Nome, T)))
                        Next
                    End If
                Case Fase.Liquid1
                    key = "3"
                    If CInt(Me.Parameters("PP_USE_EOS_LIQDENS")) = 1 Then
                        partvol = Me.m_pr.CalcPartialVolume(T, P, RET_VMOL(phase), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VTB(), "L", 0.01)
                    Else
                        partvol = New ArrayList
                        For Each subst As ClassesBasicasTermodinamica.Substancia In Me.CurrentMaterialStream.Fases(key).Componentes.Values
                            partvol.Add(1 / 1000 * subst.ConstantProperties.Molar_Weight / Me.m_props.liq_dens_rackett(T, subst.ConstantProperties.Critical_Temperature, subst.ConstantProperties.Critical_Pressure, subst.ConstantProperties.Acentric_Factor, subst.ConstantProperties.Molar_Weight, subst.ConstantProperties.Z_Rackett, P, Me.AUX_PVAPi(subst.Nome, T)))
                        Next
                    End If
                Case Fase.Liquid2
                    key = "4"
                    If CInt(Me.Parameters("PP_USE_EOS_LIQDENS")) = 1 Then
                        partvol = Me.m_pr.CalcPartialVolume(T, P, RET_VMOL(phase), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VTB(), "L", 0.01)
                    Else
                        partvol = New ArrayList
                        For Each subst As ClassesBasicasTermodinamica.Substancia In Me.CurrentMaterialStream.Fases(key).Componentes.Values
                            partvol.Add(1 / 1000 * subst.ConstantProperties.Molar_Weight / Me.m_props.liq_dens_rackett(T, subst.ConstantProperties.Critical_Temperature, subst.ConstantProperties.Critical_Pressure, subst.ConstantProperties.Acentric_Factor, subst.ConstantProperties.Molar_Weight, subst.ConstantProperties.Z_Rackett, P, Me.AUX_PVAPi(subst.Nome, T)))
                        Next
                    End If
                Case Fase.Liquid3
                    key = "5"
                    If CInt(Me.Parameters("PP_USE_EOS_LIQDENS")) = 1 Then
                        partvol = Me.m_pr.CalcPartialVolume(T, P, RET_VMOL(phase), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VTB(), "L", 0.01)
                    Else
                        partvol = New ArrayList
                        For Each subst As ClassesBasicasTermodinamica.Substancia In Me.CurrentMaterialStream.Fases(key).Componentes.Values
                            partvol.Add(1 / 1000 * subst.ConstantProperties.Molar_Weight / Me.m_props.liq_dens_rackett(T, subst.ConstantProperties.Critical_Temperature, subst.ConstantProperties.Critical_Pressure, subst.ConstantProperties.Acentric_Factor, subst.ConstantProperties.Molar_Weight, subst.ConstantProperties.Z_Rackett, P, Me.AUX_PVAPi(subst.Nome, T)))
                        Next
                    End If
                Case Fase.Vapor
                    partvol = Me.m_pr.CalcPartialVolume(T, P, RET_VMOL(phase), RET_VKij(), RET_VTC(), RET_VPC(), RET_VW(), RET_VTB(), "V", 0.01)
                    key = "2"
                Case Fase.Solid
                    partvol = New ArrayList
                    For Each subst As ClassesBasicasTermodinamica.Substancia In Me.CurrentMaterialStream.Fases(key).Componentes.Values
                        partvol.Add(0.0#)
                    Next
            End Select

            i = 0
            For Each subst As ClassesBasicasTermodinamica.Substancia In Me.CurrentMaterialStream.Fases(key).Componentes.Values
                subst.PartialVolume = partvol(i)
                i += 1
            Next

        End Sub

        Public Overrides Function AUX_VAPDENS(ByVal T As Double, ByVal P As Double) As Double
            Dim val As Double
            val = m_pr.Z_PR(T, P, RET_VMOL(Fase.Vapor), RET_VKij(), RET_VTC, RET_VPC, RET_VW, "V")
            val = (8.314 * val * T / P)
            If CInt(Me.Parameters("PP_USE_EOS_VOLUME_SHIFT")) = 1 Then
                val -= Me.AUX_CM(Fase.Vapor)
            End If
            val = 1 / val * Me.AUX_MMM(Fase.Vapor) / 1000
            Return val
        End Function

        Public Overrides Function DW_CalcFugCoeff(ByVal Vx As System.Array, ByVal T As Double, ByVal P As Double, ByVal st As State) As Double()

            DWSIM.App.WriteToConsole(Me.ComponentName & " fugacity coefficient calculation for phase '" & st.ToString & "' requested at T = " & T & " K and P = " & P & " Pa.", 2)
            DWSIM.App.WriteToConsole("Compounds: " & Me.RET_VNAMES.ToArrayString, 2)
            DWSIM.App.WriteToConsole("Mole fractions: " & Vx.ToArrayString(), 2)

            Dim prn As New PropertyPackages.ThermoPlugs.PR

            Dim lnfug As Object

            If st = State.Liquid Then
                lnfug = prn.CalcLnFug(T, P, Vx, Me.RET_VKij, Me.RET_VTC, Me.RET_VPC, Me.RET_VW, Nothing, "L")
            Else
                lnfug = prn.CalcLnFug(T, P, Vx, Me.RET_VKij, Me.RET_VTC, Me.RET_VPC, Me.RET_VW, Nothing, "V")
            End If

            Dim n As Integer = UBound(lnfug)
            Dim i As Integer
            Dim fugcoeff(n) As Double

            For i = 0 To n
                fugcoeff(i) = Exp(lnfug(i))
            Next

            DWSIM.App.WriteToConsole("Result: " & fugcoeff.ToArrayString(), 2)

            Return fugcoeff

        End Function

    End Class

End Namespace

