FROM microsoft/aspnet

RUN mkdir C:\wwwroot\sqlformat

# configure the new site in IIS
RUN powershell -NoProfile -Command \
    Import-module IISAdministration; \
    New-IISSite -Name "SqlFormat" -PhysicalPath C:\wwwroot\sqlformat -BindingInformation "*:8000:"

# listen on port 8000. 
EXPOSE 8000

# copy the publish folder to container
ADD ./publish/ /wwwroot/sqlformat