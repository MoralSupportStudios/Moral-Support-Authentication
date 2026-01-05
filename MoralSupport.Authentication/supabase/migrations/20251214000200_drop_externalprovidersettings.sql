-- Remove unused ExternalProviderSettings table now that providers are stored in secrets.
drop table if exists public."ExternalProviderSettings";
