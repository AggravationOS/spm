all:
	dotnet publish -r linux-x64 -p:PublishSingleFile=true --self-contained=true -c Release
install:
	make all
	cp bin/Release/netcoreapp6.0/linux-x64/publish/spm /usr/bin
	chmod +x /usr/bin/spm
