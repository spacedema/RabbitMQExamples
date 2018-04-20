# RabbitMQExamples

## 1. ProducerConsumer
Simple work queue(aka: Task Queue). Each task is delivered to exactly one worker <br />
- Round-robin: by default, RabbitMQ sends each message to the next consumer in sequence. On average every consumer will get the same number of messages. This way of distributing messages is called round-robin <br />
- Message acknowledgment: an ack(nowledgement) is sent back by the consumer to tell RabbitMQ that a particular message has been received, processed and that RabbitMQ is free to delete it. If a consumer dies (its channel is closed, connection is closed, or TCP connection is lost) without sending an ack, RabbitMQ will will re-queue it. If there are other consumers online at the same time, it will then quickly redeliver it to another consumer<br />
- Message durability: marking messages as persistent ~~fully guarantee~~ that a message won't be lost()<br />
- Fair dispatch: RabbitMQ don't dispatch a new message to a worker until it has processed and acknowledged the previous one. Instead, it will dispatch it to the next worker that is not still busy<br />
![alt text](https://github.com/spacedema/RabbitMQExamples/blob/master/ProducerConsumer/consumerProducer.png)

## 2. PublisherSubscriber
Example shows how we can deliver a message to multiple consumers. Dummy broadcasting using fanout exchange<br />
![alt text](https://github.com/spacedema/RabbitMQExamples/blob/master/PublisherSubscriber/fanout.png)

## 3. Routing
Example show how we can subscribe to a subset of the messages using direct exchange. The routing algorithm is simple - a message goes to the queues whose binding key equals routing key of the message<br />
![alt text](https://github.com/spacedema/RabbitMQExamples/blob/master/Routing/directExchange.png)

## 4. Topics
Example shows how we can do routing based on miltiple criteria using topics exchange. The algorithm is similar to a direct one - a message sent with a particular routing key will be delivered to all the queues that are bound with a matching binding key. However there are two important special cases for binding keys:<br />
- \* (star) can substitute for exactly one word<br />
- \# (hash) can substitute for zero or more words<br />
![alt text](https://github.com/spacedema/RabbitMQExamples/blob/master/Topics/topicExchange.png)

## 5. Rpc
Example shows how can we use RabbitMQ to build an RPC system: a client and a scalable RPC server.Dummy RPC service that returns Factorial. RPC client sends an RPC request to RPC Server and blocks until the answer is received:<br />
![alt text](https://github.com/spacedema/RabbitMQExamples/blob/master/Rpc/rpc.png)
