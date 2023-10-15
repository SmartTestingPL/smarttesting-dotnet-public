var request = require('request');

request.post(
    'http://localhost:9876/fraud/fraudCheck',
    {
        json: {
            "Guid": "89c878e3-38f7-4831-af6c-c3b4a0669022",
            "Person": {
                "Name": "Stefania",
                "Surname": "Stefanowska",
                "DateOfBirth": "2020-01-01",
                "Gender": "Female",
                "NationalIdentificationNumber": "1234567890"
            }
        }
    },
    function (error, response, body) {
        console.log(response === undefined ? "undefined" : response.statusCode)
    }
);
