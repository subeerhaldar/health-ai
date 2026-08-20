const signalR = require("@microsoft/signalr");

async function testSignalR() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5000/orchestrator/chatHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("ReceiveMessage", (message, sourceService) => {
        console.log("\n=== Received Response ===");
        console.log("Source Service:", sourceService);
        console.log("Message:", message);
        console.log("=========================\n");
        process.exit(0);
    });

    try {
        await connection.start();
        console.log("Connected to SignalR Hub successfully!");

        console.log("Sending message...");
        await connection.invoke("SendMessage", "p-123", "patient", "What are John Doe's lab results?");
    } catch (err) {
        console.error("SignalR Error:", err);
        process.exit(1);
    }
}

testSignalR();
