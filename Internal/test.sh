#!/bin/bash

if [ "$#" -eq 1 ]; then
    export MYSQL_PORT="$1"
fi

echo '=========== Testing MySql.Data ====================='
cd MySQL.Data/tests
dotnet restore 
dotnet xunit -framework netcoreapp1.1 -parallel none -xml netcore-test-results.xml
cd ../..

echo '============== Testing EF Core ======================'
cd EntityFrameworkCore/tests/MySql.EntityFrameworkCore.Basic.Tests
dotnet restore
dotnet xunit -framework netcoreapp1.1 -xml netcore-test-results.xml

cd ../MySql.EntityFrameworkCore.Design.Tests/
dotnet restore
dotnet xunit -framework netcoreapp1.1 -xml netcore-test-results.xml

cd ../MySql.EntityFrameworkCore.Migrations.Tests
dotnet restore
dotnet xunit -framework netcoreapp1.1 -xml netcore-test-results.xml

cd ../../..
