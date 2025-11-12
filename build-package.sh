#!/bin/bash

# This script builds and packages the DSearch library for NuGet

echo "=========================================="
echo "Building DSearch NuGet Package"
echo "=========================================="

cd "$(dirname "$0")/DynamicSearch"

echo ""
echo "Cleaning previous builds..."
dotnet clean -c Release

echo ""
echo "Restoring dependencies..."
dotnet restore

echo ""
echo "Building project in Release mode..."
dotnet build -c Release --no-restore

echo ""
echo "Running tests..."
cd ../DynamicSearch.Tests
dotnet test -c Release --no-build --verbosity normal
TEST_RESULT=$?

if [ $TEST_RESULT -ne 0 ]; then
    echo ""
    echo "Tests failed! Please fix the tests before packaging."
    exit 1
fi

echo ""
echo "Creating NuGet package..."
cd ../DynamicSearch
dotnet pack -c Release --no-build --output ../nupkgs

echo ""
echo "=========================================="
echo "Package build completed successfully!"
echo "=========================================="
echo ""


