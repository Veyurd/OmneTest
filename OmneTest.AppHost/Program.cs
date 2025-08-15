var builder = DistributedApplication.CreateBuilder(args);


var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .WithImageTag("16")
    .WithDataVolume();

var db = postgres.AddDatabase("OmneDB");


var apiService = builder.AddProject<Projects.OmneTest_ApiService>("apiservice")
    .WithReference(db);

builder.AddProject<Projects.OmneTest_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
