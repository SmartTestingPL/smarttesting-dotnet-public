var request = require('request');

request.post(
    'http://localhost:9876/fraud/fraudCheck',
    {
        json: {
            "Guid": "cc8aa8ff-40ff-426f-bc71-5bb7ea644108",
            "Person": {
                "Name": "Fraudeusz",
                "Surname": "Fraudowski",
                "DateOfBirth": "1980-01-01",
                "Gender": "Male",
                "NationalIdentificationNumber": "2345678901"
            }
        }
    },
    function (error, response, body) {
        console.log(response === undefined ? "undefined" : response.statusCode)
    }
);