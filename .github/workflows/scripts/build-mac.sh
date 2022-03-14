#!/bin/bash

CURRENT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

function ChangeVersion
{
    /usr/libexec/PlistBuddy -c "Set :CFBundleShortVersionString $1" "${MAC_PROJECT_DIR}/Info.plist"
}

function Notarize
{
    xcrun notarytool submit "${BUILD_OUTPUT_DIRECTORY}/${ZIP_NAME}" \
                    --apple-id "${APPLE_ID}" \
                    --team-id "${APPLE_TEAM_ID}" \
                    --password "${APPLE_ID_PASSWORD}" \
                    --wait
}

function Build
{
    msbuild "${MAC_PROJECT_DIR}/$MAC_PROJECT_FILE" /t:rebuild /property:Configuration=Release;

    Error=$?
    if [ "$Error" != "0" ]; then exit $Error; fi
}

function BuildZip
{
    pushd "$MAC_PROJECT_DIR/bin/Release/";
        zip -r "$BUILD_OUTPUT_DIRECTORY/${ZIP_NAME}" "$MAC_APP_FILE";
    popd;
}

function ClearBuildDirectory
{
    rm -rf "$BUILD_OUTPUT_DIRECTORY";
    mkdir -p "$BUILD_OUTPUT_DIRECTORY";
}

function Start
{
    ClearBuildDirectory;
    Error=$?
    if [ "$Error" != "0" ]; then exit $Error; fi

    Build;
    Error=$?
    if [ "$Error" != "0" ]; then exit $Error; fi

    BuildZip;
    Error=$?
    if [ "$Error" != "0" ]; then exit $Error; fi

    Notarize;
    Error=$?
    if [ "$Error" != "0" ]; then exit $Error; fi
}

function Entry
{
    if [ "$1" != "" ]; then
        ChangeVersion $1;
        export VERSION="$1"
        export ZIP_NAME="${PROJECT_NAME}.zip"
    else
        export ZIP_NAME="${PROJECT_NAME}.zip"
    fi

    Start;
    Error=$?

    if [ "$1" != "" ]; then
        ChangeVersion "$1-development";
    fi

    if [ "$Error" != "0" ]; then exit $Error; fi
}


Entry $@;