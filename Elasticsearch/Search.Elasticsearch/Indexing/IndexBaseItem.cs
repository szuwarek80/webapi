using Nest;
using Search.Elasticsearch.Mapping;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Search.Elasticsearch.Indexing
{
    public abstract class IndexBaseItem
    {
        public abstract string Name { get; }

        public abstract Task<CreateIndexResponse> CreateIndex(IElasticClient aClient);

        public abstract Task<BulkResponse> InsertDataIntoIndex(IElasticClient aClient, IDataProvider aDataProvider);


        public async Task<DeleteIndexResponse> DeleteIndex(IElasticClient aClient)
        {
            var res = await aClient.Indices.ExistsAsync(this.Name);

            if (res.Exists)
                return await aClient.Indices.DeleteAsync(this.Name);

            return null;
        }

        protected virtual async Task<BulkResponse> InsertDataIntoIndex<T>(IElasticClient aClient, List<T> aData)
            where T : SearchableBaseItem
        {
            var bulkResponse = await aClient.BulkAsync(b => b
                    .IndexMany(aData, (op, item) => op
                        .Index(this.Name)
                    )
                );

            return bulkResponse;
        }

        protected static IPromise<IIndexSettings> InitCommonIndexSettingsDescriptor(IndexSettingsDescriptor aDescriptor)
        {
            return aDescriptor
                .NumberOfReplicas(0)
                .NumberOfShards(1)
                .Analysis(InitCommonAnalyzers);
        }

        protected static IAnalysis InitCommonAnalyzers(AnalysisDescriptor analysis)
        {
            return analysis.Analyzers(a => a
                .Custom("autocomplete", ca => ca
                    .Filters("eng_stopwords", "trim", "lowercase")
                    .Tokenizer("autocomplete")
                )
                .Custom("autocomplete_search", ca => ca
                   .Filters("eng_stopwords", "trim", "lowercase")
                   .Tokenizer("lowercase")
                )
            )
            .Tokenizers(tdesc => tdesc               
                .EdgeNGram("autocomplete", e => e
                    .MinGram(3)
                    .MaxGram(15)
                    .TokenChars(TokenChar.Letter, TokenChar.Digit)
                )
            )
            .TokenFilters(f => f                
                .Stop("eng_stopwords", lang => lang                    
                    .StopWords("_english_")
                )
            );
        }

    }
}
