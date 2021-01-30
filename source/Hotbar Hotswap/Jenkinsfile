#!/usr/bin/env groovy
import groovy.json.JsonOutput

// Has to be the same as the solution name!
def modName = 'HotbarHotswap'
// Needed to map jenkins config and nuget store to the container
def jenkinsHome = "/var/lib/jenkins"
// Path to the game, needed for build, mapped to /pot/stardewvalley in the csproj file
def gamePath = "/opt/stardewvalley"

pipeline {
    agent {
        docker {
            image 'mono:6.12.0.107'
            args "-v $gamePath:/opt/stardewvalley -v $jenkinsHome/.config/:/.config/:rw,z -v $jenkinsHome/.nuget/:/.nuget/:rw,z"
        }
    }

    stages {
        stage("Clean") {
            steps {
                script {
                    println 'Cleaning build directory'
                    sh(script: "rm -rf build/")
                }

            }
        }
        stage("Preflight") {
            steps {
                script {
                    println 'Starting preflight'
                    sh(script: "sed -i 's/%build_number%/$env.BUILD_NUMBER/g' build.json")
                    def buildConfig = readJSON file: "build.json"
                    def version = buildConfig["version"]
                    sh(script: "sed -i 's/1\\.0\\.0-no-op/$version/g' '$modName/manifest.json'")
                }

            }
        }
        stage('Build') {
            steps {
                echo 'Building'
                sh "nuget restore '${modName}.sln'"
                sh "msbuild -p:EnableModDeploy=false -p:GamePath=$gamePath -p:ModZipPath=../build/"
            }
        }

        stage('Deploy') {
            steps {
                withCredentials([file(credentialsId: 'mod_build_secrets', variable: 'SECRET_FILE')]) {
                    script {
                        final String url = "https://stardewvalley.curseforge.com"
                        def manifest = [
                                changelog    : "",
                                changelogType: "",
                                displayName  : "",
                                gameVersions : [],
                                releaseType  : ""
                        ]

                        def buildConfig = readJSON file: "build.json"
                        if (buildConfig.containsKey("relations") && buildConfig['relations'].size() > 0) {
                            manifest = [
                                    changelog    : "",
                                    changelogType: "",
                                    displayName  : "",
                                    gameVersions : [],
                                    releaseType  : "",
                                    relations    : [
                                            projects: []
                                    ]
                            ]
                        }
                        def deploy = buildConfig["deploy"]
                        if (deploy) {
                            manifest["changelog"] = readFile(file: buildConfig["changelogFile"])
                            manifest['changelogType'] = buildConfig["changelogType"]
                            manifest["releaseType"] = buildConfig["releaseType"]

                            if (buildConfig.containsKey("relations") && buildConfig['relations'].size() > 0)
                                manifest["relations"]["projects"] = buildConfig['relations']

                            def secrets = readJSON file: SECRET_FILE
                            def projectId = buildConfig["curseProjectId"];
                            def gameVersions = buildConfig["gameVersions"];
                            def curseApiKey = secrets["curseApiKey"]
                            final String versionRequest = sh(script: "set +x && curl -s $url/api/game/versions -H 'X-Api-Token: $curseApiKey'", returnStdout: true).trim()
                            def versionList = readJSON text: versionRequest
                            def curseVersions = versionList.findAll {
                                for (String version : gameVersions) {
                                    if (it.name == version) {
                                        return true;
                                    }
                                }
                            }.collect {
                                it.id
                            };
                            println "Found curse versions: $curseVersions"
                            manifest.gameVersions = curseVersions;
                            final String fileName = sh(script: "ls build/", returnStdout: true).trim()
                            manifest["displayName"] = fileName

                            def json = JsonOutput.toJson(manifest)
                            println "Uploading"
                            final String response = sh(script: "set +x && curl -s $url/api/projects/$projectId/upload-file -H 'X-Api-Token: $curseApiKey' -H 'content-type: multipart/form-data;' --form 'metadata=$json' --form 'file=@build/$fileName'", returnStdout: true).trim()
                            println "Upload Complete"
                        } else {
                            println "Deploy disabled in build.json!"
                        }

                    }
                }
            }
        }
    }
    post {
        always {
            archiveArtifacts 'build/**'
        }
    }
}