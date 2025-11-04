import * as signalR from "@microsoft/signalr";
import type { AlertDto } from "./api";

const HUB_URL = process.env.NEXT_PUBLIC_SIGNALR_URL || "https://localhost:5001/hubs/alerts";

export class AlertHubConnection {
  private connection: signalR.HubConnection | null = null;
  private onAlertReceived: ((alert: AlertDto) => void) | null = null;

  async start(token: string, onAlert: (alert: AlertDto) => void) {
    this.onAlertReceived = onAlert;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${HUB_URL}?access_token=${token}`, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.connection.on("ReceiveAlert", (alert: AlertDto) => {
      console.log("Alert received via SignalR:", alert);
      if (this.onAlertReceived) {
        this.onAlertReceived(alert);
      }
    });

    try {
      await this.connection.start();
      console.log("SignalR Connected!");
    } catch (err) {
      console.error("SignalR Connection Error:", err);
      setTimeout(() => this.start(token, onAlert), 5000);
    }
  }

  async stop() {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
    }
  }

  getConnectionState(): signalR.HubConnectionState {
    return this.connection?.state ?? signalR.HubConnectionState.Disconnected;
  }
}

export const alertHub = new AlertHubConnection();
