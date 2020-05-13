#!/bin/bash
dotnet build src/CommandLine/ -c Release --version-suffix 2.9.0-rmunn-1 /p:BuildTarget=base
dotnet pack src/CommandLine/ -c Release --version-suffix 2.9.0-rmunn-1 /p:BuildTarget=base
