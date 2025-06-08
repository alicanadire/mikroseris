// ToyStore MongoDB Initialization
// Notification Service için koleksiyonlar ve sample data

// Notifications koleksiyonu
db.createCollection("notifications", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["type", "recipient", "message", "status", "createdAt"],
      properties: {
        type: {
          bsonType: "string",
          enum: ["email", "sms", "push"],
        },
        recipient: {
          bsonType: "string",
        },
        message: {
          bsonType: "object",
          required: ["subject", "body"],
          properties: {
            subject: { bsonType: "string" },
            body: { bsonType: "string" },
            templateId: { bsonType: "string" },
          },
        },
        status: {
          bsonType: "string",
          enum: ["pending", "sent", "failed", "delivered"],
        },
        metadata: {
          bsonType: "object",
        },
        createdAt: {
          bsonType: "date",
        },
        sentAt: {
          bsonType: "date",
        },
      },
    },
  },
});

// Logs koleksiyonu
db.createCollection("logs", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["level", "message", "timestamp"],
      properties: {
        level: {
          bsonType: "string",
          enum: ["info", "warning", "error", "debug"],
        },
        message: {
          bsonType: "string",
        },
        service: {
          bsonType: "string",
        },
        userId: {
          bsonType: "string",
        },
        metadata: {
          bsonType: "object",
        },
        timestamp: {
          bsonType: "date",
        },
      },
    },
  },
});

// Index'ler
db.notifications.createIndex({ recipient: 1 });
db.notifications.createIndex({ status: 1 });
db.notifications.createIndex({ createdAt: -1 });
db.notifications.createIndex({ type: 1, status: 1 });

db.logs.createIndex({ level: 1 });
db.logs.createIndex({ timestamp: -1 });
db.logs.createIndex({ service: 1, timestamp: -1 });

// Sample notifications
db.notifications.insertMany([
  {
    type: "email",
    recipient: "admin@toystore.com",
    message: {
      subject: "Welcome to ToyStore!",
      body: "Thank you for setting up ToyStore microservices.",
      templateId: "welcome",
    },
    status: "sent",
    metadata: {
      source: "system",
      priority: "normal",
    },
    createdAt: new Date(),
    sentAt: new Date(),
  },
  {
    type: "email",
    recipient: "customer@toystore.com",
    message: {
      subject: "Order Confirmation",
      body: "Your order has been confirmed and is being processed.",
      templateId: "order_confirmation",
    },
    status: "sent",
    metadata: {
      orderId: "ORDER-001",
      source: "order_service",
    },
    createdAt: new Date(),
    sentAt: new Date(),
  },
]);

// Sample logs
db.logs.insertMany([
  {
    level: "info",
    message: "MongoDB notification service initialized",
    service: "notification-service",
    timestamp: new Date(),
    metadata: {
      version: "1.0.0",
      environment: "development",
    },
  },
  {
    level: "info",
    message: "Database collections created successfully",
    service: "notification-service",
    timestamp: new Date(),
    metadata: {
      collections: ["notifications", "logs"],
    },
  },
]);

print("MongoDB notification database initialized successfully!");
