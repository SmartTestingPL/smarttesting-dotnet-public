Describe "Script" {

    # Whitebox testing
    It "should invoke a request to an external website" {
        Mock Invoke-RestMethod {
            "invoke rest method $uri"
        }

        . "../src/Script.ps1"

        Assert-MockCalled Invoke-RestMethod -Times 1 -ParameterFilter {$uri -eq "https://reqres.in/api/users/2"}
    }

    # Blackbox testing
    It "should return a response from an external website" {
        Mock Invoke-RestMethod {
            "invoke rest method $uri"
        }

        $testUri = "http://example.com"

        (. "../src/Script.ps1" $testUri) | Should -Be "`"invoke rest method $testUri/`""
    }

    # Failure testing
    It "should not fail when request retrieval failed" {
        Mock Invoke-RestMethod {
            throw
        }

        $testUri = "http://example.com"

        (. "../src/Script.ps1" $testUri) | Should -BeLike "Failed*"
    }
}
  