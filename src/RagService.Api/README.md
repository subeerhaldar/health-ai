# RagService.Api

The RAG Service coordinates document embedding (ingestion) and similarity searches (retrieval) against a PostgreSQL vector database (`pgvector`).

> **IMPORTANT: Vector Dimension Change**  
> We recently refactored the application to use OpenRouter's `liquid/lfm2.5-embedding-350m:free` model, which outputs embeddings at `1024` dimensions. The EF Core column `Documents.Embedding` has been migrated from `vector(1536)` to `vector(1024)`. 
> 
> **Any previously ingested documents from the old model must be re-ingested**, as their old 1536-dimensional vectors are incompatible with the new column constraint and will cause errors or be dropped by Postgres.
