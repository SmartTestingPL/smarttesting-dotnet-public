import org.springframework.cloud.contract.spec.Contract

Contract.make {
	request {
		method POST()
		url "/fraud/fraudCheck"
		headers {
			contentType applicationJson()
		}
		body('''
				{
				  "Guid" : "cc8aa8ff-40ff-426f-bc71-5bb7ea644108",
				  "Person" : {
					"Name" : "Fraudeusz",
					"Surname" : "Fraudowski",
					"DateOfBirth" : "1980-01-01",
					"Gender" : "Male",
					"NationalIdentificationNumber" : "2345678901"
				  }
				}
				''')
	}
	response {
		status UNAUTHORIZED()
	}

}