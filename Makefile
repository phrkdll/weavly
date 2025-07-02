.PHONY: help

help: # Display this help screen
	@echo "Targets:"
	@echo
	@sed -n 's/^\([A-Za-z0-9_.-]*\):.*# \(.*\)$$/\1: \2/p' Makefile | sort | column -t -s ':'
	@echo

build: # Perform "dotnet build ."
	@dotnet build .
tool: uninstall clean pack.cli install # Repack and reinstall Weavly.Cli

run: # Run the sample project
	@dotnet run --project ./Weavly.Api
watch: # Watch the sample project
	@dotnet watch --project ./Weavly.Api
check: # Perform "csharpier check ."
	@csharpier check .
format: # Perform "csharpier format ."
	@csharpier format .

clean: # Perform "dotnet clean ."
	@dotnet clean .
clean-sym: # Performs a full solution cleanup with bin/obj removal
	@dotnet msbuild -t:FullClean
pack-cli: # Perform "dotnet pack ./Weavly.Cli"
	@dotnet pack ./Weavly.Cli
pack: # Perform "dotnet pack ."
	@dotnet pack .
install: # Install Weavly.Cli as a global tool
	@dotnet tool install -g --add-source ./Weavly.Cli/nupkg Weavly.Cli
uninstall: # Uninstall Weavly.Cli
	-@dotnet tool uninstall -g Weavly.Cli ||: