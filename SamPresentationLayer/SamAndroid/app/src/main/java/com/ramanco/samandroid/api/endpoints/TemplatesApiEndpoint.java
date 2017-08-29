package com.ramanco.samandroid.api.endpoints;

import com.ramanco.samandroid.api.dtos.TemplateDto;
import com.ramanco.samandroid.consts.ApiActions;

import retrofit2.Call;
import retrofit2.http.GET;

public interface TemplatesApiEndpoint {

    @GET(ApiActions.templates_all)
    Call<TemplateDto[]> all();

}
