Feature: Create Order Saga
Saga for creating an order

@sagas
Scenario: Create an order with a saga
	Given a saga configuration has been created with 'sagaconfig.json'
	And a customer has been created with 'customer.json'
	When I make a POST request with 'order.json' to 'api/order'
	Then the response status code is '201'
	And the location header is 'api/order/id'
	And the response entity should be 'order.json'
	And the customer credit should equal 12.5
	And the order state should be 'Created'

@sagas
Scenario: Create an order with a saga that rolls back due to insufficient customer credit
	Given a saga configuration has been created with 'sagaconfig.json'
	And a customer has been created with 'customer.json'
	And the customer credit is 5.0
	When I make a POST request with 'order.json' to 'api/order'
	Then the response status code is '201'
	And the location header is 'api/order/id'
	And the response entity should be 'order.json'
	And the customer credit should equal 5.0
	And the order state should be 'Initial'
