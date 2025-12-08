import "./globals.css";
import React from "react";

export const metadata = {
  title: "CareLytix Health Analytics",
  description: "Sample Lead C# Developer portfolio project"
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body className="min-h-screen">
        <div className="mx-auto max-w-6xl px-4 py-6">
          <header className="mb-6 flex items-center justify-between">
            <h1 className="text-2xl font-bold">CareLytix Health Analytics</h1>
            <nav className="space-x-4 text-sm">
              <a href="/">Dashboard</a>
              <a href="/patients">Patients</a>
              <a href="/ai-assistant">AI Assistant</a>
            </nav>
          </header>
          {children}
        </div>
      </body>
    </html>
  );
}
