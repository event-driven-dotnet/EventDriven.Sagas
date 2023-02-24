Feature: Create Order Saga
Saga for creating an order

@sagas
Scenario: Create an order with a saga
	Given a saga configuration has been created with 'sagaconfig.json'
	And a customer has been created with 'customer.json'
	And the customer credit is 100.0
	And inventory has been created with 'inventory.json'
	And the inventory quantity is 9
	When I make a POST request with 'order.json' to 'api/order'
	Then the response status code is '200'
	And the response entity should be 'order.json'
	And the customer credit should equal 77.5
	And the inventory quantity should equal 6
	And the order state should be 'Created'

@sagas
Scenario: Create an order with a saga that rolls back due to insufficient customer credit
	Given a saga configuration has been created with 'sagaconfig.json'
	And a customer has been created with 'customer.json'
	And the customer credit is 20.0
	And inventory has been created with 'inventory.json'
	And the inventory quantity is 9
	When I make a POST request with 'order.json' to 'api/order'
	Then the response status code is '200'
	And the response entity should be 'order.json'
	And the customer credit should equal 20.0
	And the inventory quantity should equal 9
	And the order state should be 'Initial'

@sagas
Scenario: Create an order with a saga that rolls back due to insufficient product inventory
	Given a saga configuration has been created with 'sagaconfig.json'
	And a customer has been created with 'customer.json'
	And the customer credit is 100.0
	And inventory has been created with 'inventory.json'
	And the inventory quantity is 2
	When I make a POST request with 'order.json' to 'api/order'
	Then the response status code is '200'
	And the response entity should be 'order.json'
	And the customer credit should equal 100.0
	And the inventory quantity should equal 2
	And the order state should be 'Initial'

@sagas
Scenario: Create multiple orders with sagas
	Given a saga configuration has been created with 'sagaconfig.json'
	And a customer has been created with 'customer.json'
	And the customer credit is 100.0
	And inventory has been created with 'inventory.json'
	And the inventory quantity is 9
	When I make POST requests with 'order1.json,order2.json,order3.json' to 'api/order'
	Then the response status codes are '200'
	And the response entities should be 'order1.json,order2.json,order3.json'
	And the customer credit should equal 32.5
	And the inventory quantity should equal 0
	And the order states should be 'Created'

@sagas
Scenario: Create multiple orders with sagas that roll back due to insufficient customer credit
	Given a saga configuration has been created with 'sagaconfig.json'
	And a customer has been created with 'customer.json'
	And the customer credit is 20.0
	And inventory has been created with 'inventory.json'
	And the inventory quantity is 9
	When I make POST requests with 'order1.json,order2.json,order3.json' to 'api/order'
	Then the response status codes are '200'
	And the response entities should be 'order1.json,order2.json,order3.json'
	And the customer credit should equal 20.0
	And the inventory quantity should equal 9
	And the order states should be 'Initial'

@sagas
Scenario: Create multiple orders with sagas that roll back due to insufficient product inventory
	Given a saga configuration has been created with 'sagaconfig.json'
	And a customer has been created with 'customer.json'
	And the customer credit is 100.0
	And inventory has been created with 'inventory.json'
	And the inventory quantity is 2
	When I make POST requests with 'order1.json,order2.json,order3.json' to 'api/order'
	Then the response status codes are '200'
	And the response entities should be 'order1.json,order2.json,order3.json'
	And the customer credit should equal 100.0
	And the inventory quantity should equal 2
	And the order states should be 'Initial'
