-- Run this once in your Supabase project's SQL Editor (Dashboard -> SQL Editor -> New query).

create table if not exists meters (
  id uuid primary key default gen_random_uuid(),
  name text not null,
  current_reading numeric not null default 0,
  previous_reading numeric not null default 0,
  base_reading numeric not null default 0,
  last_updated_by text,
  last_updated_at timestamptz default now(),
  created_at timestamptz default now()
);

create table if not exists reading_logs (
  id uuid primary key default gen_random_uuid(),
  meter_id uuid not null references meters(id) on delete cascade,
  previous_reading numeric not null,
  new_reading numeric not null,
  units_consumed numeric generated always as (new_reading - previous_reading) stored,
  updated_by text,
  updated_at timestamptz not null default now()
);

create index if not exists idx_reading_logs_meter_id on reading_logs(meter_id);

-- This app has no login system by design (simple family/home use).
-- Row Level Security is enabled with permissive policies so the anon key
-- can read/write. Do NOT reuse this schema for anything with sensitive data.
alter table meters enable row level security;
alter table reading_logs enable row level security;

create policy "public read meters" on meters for select using (true);
create policy "public insert meters" on meters for insert with check (true);
create policy "public update meters" on meters for update using (true);
create policy "public delete meters" on meters for delete using (true);

create policy "public read reading_logs" on reading_logs for select using (true);
create policy "public insert reading_logs" on reading_logs for insert with check (true);
