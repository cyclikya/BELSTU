pipeline {
    agent any

    stages {
        stage('Build') {
            steps {
                bat 'mvn compile'
            }
        }

        stage('Static Analysis') {
            steps {
                bat 'mvn checkstyle:check'
            }
        }

        stage('Test') {
            steps {
                bat 'mvn test'
            }
        }

        stage('Dependencies') {
            steps {
                bat 'mvn dependency:tree'
            }
        }

        stage('Package') {
            steps {
                bat 'mvn package -DskipTests'
            }
        }
    }
}
