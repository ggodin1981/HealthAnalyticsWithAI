"use client";

import { useState } from "react";
import axios from "axios";

type AnalyzeResponse = {
  answer: string;
  model: string;
};

export default function AiAssistantPage() {
  const [question, setQuestion] = useState("");
  const [mrn, setMrn] = useState("");
  const [answer, setAnswer] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const ask = async () => {
    setLoading(true);
    setAnswer(null);
    try {
      const token = window.localStorage.getItem("access_token");
      const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? "https://localhost:41624";
      const res = await axios.post<AnalyzeResponse>(
        `${baseUrl}/api/ai/analyze`,
        {
          question,
          patientMrn: mrn || null,
          includePopulationStats: true
        },
        {
          headers: token
            ? {
                Authorization: `Bearer ${token}`
              }
            : undefined
        }
      );
      setAnswer(res.data.answer);
    } catch (err) {
      console.error("AI call failed", err);
      setAnswer(
        "Error calling AI assistant. Make sure the API is running and Ai:Endpoint is configured."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <main className="space-y-4">
      <h2 className="text-xl font-semibold">AI Analytics Assistant</h2>
      <p className="text-sm text-slate-300">
        Ask questions about patient cohorts, risk trends, or utilization. The backend will
        join EHR-style data with an LLM to generate narrative insights. Demo requires you
        to login first (e.g. via Patients &quot;Demo Login&quot;).
      </p>

      <div className="space-y-3 rounded-lg border border-slate-800 bg-slate-900/60 p-4">
        <div className="flex flex-col gap-3 md:flex-row">
          <div className="flex-1">
            <label className="mb-1 block text-xs text-slate-400">Question</label>
            <textarea
              className="w-full rounded-md border border-slate-700 bg-slate-950 px-3 py-2 text-sm"
              rows={3}
              placeholder="e.g. What are the key risk drivers for this patient cohort over the last 12 months?"
              value={question}
              onChange={e => setQuestion(e.target.value)}
            />
          </div>
          <div className="w-full md:w-60">
            <label className="mb-1 block text-xs text-slate-400">
              Patient MRN (optional)
            </label>
            <input
              className="w-full rounded-md border border-slate-700 bg-slate-950 px-3 py-2 text-sm"
              placeholder="MRN for patient-level insight"
              value={mrn}
              onChange={e => setMrn(e.target.value)}
            />
          </div>
        </div>
        <button
          onClick={ask}
          disabled={loading}
          className="rounded-md bg-emerald-500 px-4 py-2 text-sm font-semibold text-slate-950 hover:bg-emerald-400 disabled:opacity-50"
        >
          {loading ? "Asking AI..." : "Ask AI Analyst"}
        </button>
      </div>

      {answer && (
        <div className="rounded-lg border border-slate-800 bg-slate-900/60 p-4">
          <h3 className="mb-2 text-sm font-semibold text-emerald-400">
            Assistant Response
          </h3>
          <p className="whitespace-pre-wrap text-sm text-slate-100">{answer}</p>
        </div>
      )}
    </main>
  );
}
