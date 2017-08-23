package com.ramanco.samandroid.fragments;


import android.app.ProgressDialog;
import android.content.Intent;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.Toast;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.activities.CitySelectionActivity;
import com.ramanco.samandroid.adapters.PairAdapter;
import com.ramanco.samandroid.api.dtos.MosqueDto;
import com.ramanco.samandroid.api.endpoints.MosquesApiEndpoint;
import com.ramanco.samandroid.exceptions.CallServerException;
import com.ramanco.samandroid.objects.KeyValuePair;
import com.ramanco.samandroid.utils.ApiUtil;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.PrefUtil;
import com.ramanco.samandroid.utils.UxUtil;

import java.io.IOException;

import retrofit2.Call;
import retrofit2.Response;

public class SendConsolationFragment extends Fragment {

    //region Ctors:
    public SendConsolationFragment() {
    }
    //endregion

    //region Fields:
    private  MosqueDto mosque;
    //endregion

    //region Overrides:
    @Override
    public void onStart() {
        try {
            super.onStart();

            //region check city id:
            int cityId = PrefUtil.getCityID(getActivity());
            if (cityId <= 0) {
                Intent intent = new Intent(getActivity(), CitySelectionActivity.class);
                startActivity(intent);
            }
            //endregion

        } catch (Exception ex) {
            ExceptionManager.Handle(getActivity(), ex);
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View fragmentView = inflater.inflate(R.layout.fragment_send_consolation, container, false);

        try {

            showMosqueSelectionStep();

        } catch (Exception ex) {
            ExceptionManager.Handle(getActivity(), ex);
        }

        return fragmentView;
    }
    //endregion

    //region Methods:
    private void showMosqueSelectionStep() {
        final MosqueSelectionFragment fragment = new MosqueSelectionFragment();
        fragment.setOnMosqueSelectionAction(new Runnable() {
            @Override
            public void run() {
                try {
                    mosque = fragment.getSelectedMosque();
                    showObitSelectionStep();
                } catch (Exception ex) {
                    ExceptionManager.Handle(getActivity(), ex);
                }
            }
        });
        FragmentManager fragmentManager = getActivity().getSupportFragmentManager();
        FragmentTransaction transaction = fragmentManager.beginTransaction();
        transaction.replace(R.id.fragment_placeholder, fragment);
        transaction.commit();
    }
    private void showObitSelectionStep() {
        Toast.makeText(getActivity(), "Go To Obit selection Step!", Toast.LENGTH_SHORT).show();
    }
    //endregion
}