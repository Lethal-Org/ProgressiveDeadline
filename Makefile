all: clean	build

build:
	dotnet build ProgressiveDeadline -c Release
	mkdir -p build/BepInEx/plugins
	cp README.md build
	cp CHANGELOG.md build
	cp manifest.json build
	cp icon.png build
	mv ProgressiveDeadline/bin/Release/netstandard2.1/ProgressiveDeadline.dll build/BepInEx/plugins

clean:
	dotnet clean ./ProgressiveDeadline
	rm -rf ./ProgressiveDeadline/bin
	rm -rf ./ProgressiveDeadline/obj
	rm -rf ./build

.PHONY: build
