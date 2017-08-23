package com.ramanco.samandroid.api.endpoints;

import com.ramanco.samandroid.api.dtos.MosqueDto;

import retrofit2.Call;
import retrofit2.http.GET;
import retrofit2.http.Query;

public interface MosquesApiEndpoint {

    @GET("mosques/findbycity")
    Call<MosqueDto[]> findByCity(@Query("cityid") int cityId);

}
