#!/bin/bash

CURRENT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

function ChangeVersion
{
    sed -i '$!N;s/<key>CFBundleShortVersionString<\/key>\n\t<string>.*$/<key>CFBundleShortVersionString<\/key>\n\t<string>'"$1"'<\/string>/g' "${MAC_PROJECT_DIR}/Info.plist"
}

function Notarize
{
    xcrun notarytool submit "${FULL_BUILD_OUTPUT_DIRECTORY}/${ZIP_NAME}" \
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
    cd "$MAC_PROJECT_DIR/bin/Release/"

    zip -r "$FULL_BUILD_OUTPUT_DIRECTORY/${ZIP_NAME}" "$MAC_APP_FILE"
}

function ClearBuildDirectory
{
    rm -rf "$FULL_BUILD_OUTPUT_DIRECTORY";
    mkdir "$FULL_BUILD_OUTPUT_DIRECTORY";
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
        export ZIP_NAME="${PROJECT_NAME} ${VERSION}.zip"
    else
        export ZIP_NAME="${PROJECT_NAME}.zip"
    fi

    export FULL_BUILD_OUTPUT_DIRECTORY="$BUILD_OUTPUT_DIRECTORY/macOS"

    Start;
    Error=$?

    if [ "$1" != "" ]; then
        ChangeVersion "$1-development";
    fi

    if [ "$Error" != "0" ]; then exit $Error; fi
}


Entry $@;