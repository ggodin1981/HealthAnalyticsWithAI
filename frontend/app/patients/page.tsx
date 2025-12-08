"use client";

import { useEffect, useState } from "react";
import axios from "axios";

type PatientListItem = {
  id: string;
  mrn: string;
  fullName: string;
  dateOfBirth: string;
  gender: string;
};

export default function PatientsPage() {
  const [patients, setPatients] = useState<PatientListItem[]>([]);
  const [query, setQuery] = useState("");
  const [token, setToken] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const stored = window.localStorage.getItem("access_token");
    if (stored) {
      setToken(stored);
      loadPatients(stored);
    }
  }, []);

  const loginDemo = async () => {
    try {
      setLoading(true);
      const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? "https://localhost:41624";
      const res = await axios.post(`${baseUrl}/api/auth/login`, {
        userName: "admin",
        password: "Admin123!"
      });
      window.localStorage.setItem("access_token", res.data.accessToken);
      setToken(res.data.accessToken);
      await loadPatients(res.data.accessToken);
    } catch (err) {
      console.error("Demo login failed", err);
      alert("Demo login failed. Check API is running on https://localhost:41624");
    } finally {
      setLoading(false);
    }
  };

  const loadPatients = async (bearer?: string, q?: string) => {
    try {
      setLoading(true);
      const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? "https://localhost:41624";
      const headers: any = {};
      if (bearer) {
        headers["Authorization"] = `Bearer ${bearer}`;
      }
      const res = await axios.get<PatientListItem[]>(`${baseUrl}/api/patients`, {
        params: { q },
        headers
      });
      setPatients(res.data);
    } catch (err) {
      console.error("Load patients failed", err);
      alert("Failed to load patients. Check API / token.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <main>
      <div className="mb-4 flex items-center justify-between">
        <h2 className="text-xl font-semibold">Patients</h2>
        <div className="flex gap-2">
          <input
            className="rounded-md border border-slate-700 bg-slate-900 px-3 py-1 text-sm"
            placeholder="Search by MRN or name..."
            value={query}
            onChange={e => setQuery(e.target.value)}
          />
          <button
            onClick={() => token && loadPatients(token, query)}
            className="rounded-md bg-emerald-500 px-3 py-1 text-sm font-medium text-slate-950 hover:bg-emerald-400 disabled:opacity-50"
            disabled={!token || loading}
          >
            {loading ? "Loading..." : "Search"}
          </button>
          {!token && (
            <button
              onClick={loginDemo}
              className="rounded-md bg-sky-500 px-3 py-1 text-sm font-medium text-slate-950 hover:bg-sky-400"
              disabled={loading}
            >
              {loading ? "Logging in..." : "Demo Login"}
            </button>
          )}
        </div>
      </div>

      <div className="overflow-hidden rounded-lg border border-slate-800 bg-slate-900/60">
        <table className="min-w-full text-sm">
          <thead className="bg-slate-800">
            <tr>
              <th className="px-3 py-2 text-left">MRN</th>
              <th className="px-3 py-2 text-left">Name</th>
              <th className="px-3 py-2 text-left">DOB</th>
              <th className="px-3 py-2 text-left">Gender</th>
            </tr>
          </thead>
          <tbody>
            {patients.map(p => (
              <tr key={p.id} className="border-t border-slate-800">
                <td className="px-3 py-2">{p.mrn}</td>
                <td className="px-3 py-2">{p.fullName}</td>
                <td className="px-3 py-2">
                  {new Date(p.dateOfBirth).toLocaleDateString()}
                </td>
                <td className="px-3 py-2">{p.gender}</td>
              </tr>
            ))}
            {patients.length === 0 && (
              <tr>
                <td colSpan={4} className="px-3 py-4 text-center text-slate-400">
                  {token
                    ? "No patients found."
                    : "You must login (demo) to load patients."}
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </main>
  );
}
