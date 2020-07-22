{{/* vim: set filetype=mustache: */}}
{{/*
Expand the name of the chart.
*/}}
{{- define "dpio-asset-api.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "dpio-asset-api.fullname" -}}
{{- if .Values.fullnameOverride -}}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- $name := default .Chart.Name .Values.nameOverride -}}
{{- if contains $name .Release.Name -}}
{{- .Release.Name | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" -}}
{{- end -}}
{{- end -}}
{{- end -}}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "dpio-asset-api.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Common labels
*/}}
{{- define "dpio-asset-api.labels" -}}
helm.sh/chart: {{ include "dpio-asset-api.chart" . }}
{{ include "dpio-asset-api.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end -}}

{{/*
Selector labels
*/}}
{{- define "dpio-asset-api.selectorLabels" -}}
app.kubernetes.io/name: {{ include "dpio-asset-api.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end -}}

{{/*
Create service account name.
*/}}
{{- define "dpio-asset-api.serviceAccountName" -}}
{{- $fullName := include "dpio-asset-api.fullname" . -}}
{{- printf "%s-%s" .Release.Namespace $fullName | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Create service name.
*/}}
{{- define "dpio-asset-api.serviceName" -}}
{{- if .Values.service.name -}}
{{- .Values.service.name | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- include "dpio-asset-api.fullname" . -}}
{{- end -}}
{{- end -}}