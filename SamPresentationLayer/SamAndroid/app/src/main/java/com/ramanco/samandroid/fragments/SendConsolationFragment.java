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
import android.widget.ImageButton;
import android.widget.ListView;
import android.widget.Toast;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.activities.CitySelectionActivity;
import com.ramanco.samandroid.adapters.PairAdapter;
import com.ramanco.samandroid.api.dtos.MosqueDto;
import com.ramanco.samandroid.api.dtos.ObitDto;
import com.ramanco.samandroid.api.dtos.TemplateDto;
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
    private MosqueDto selectedMosque;
    private ObitDto selectedObit;
    private TemplateDto selectedTemplate;
    private boolean nextVisible = false;
    private boolean prevVisible = false;
    private Runnable onNextClickListener;
    private Runnable onPreviousClickListener;
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

            updateNavButtonsStat();
            showMosqueSelectionStep();

            //region next prev on click:
            ImageButton btnNext = (ImageButton) fragmentView.findViewById(R.id.btn_next);
            ImageButton btnPrev = (ImageButton) fragmentView.findViewById(R.id.btn_prev);
            btnNext.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    try {
                        if (getOnNextClickListener() != null)
                            onNextClickListener.run();
                    } catch (Exception ex) {
                        ExceptionManager.Handle(getActivity(), ex);
                    }
                }
            });
            btnPrev.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View v) {
                    try {
                        if (getOnPreviousClickListener() != null)
                            onPreviousClickListener.run();
                    } catch (Exception ex) {
                        ExceptionManager.Handle(getActivity(), ex);
                    }
                }
            });
            //endregion

        } catch (Exception ex) {
            ExceptionManager.Handle(getActivity(), ex);
        }

        return fragmentView;
    }
    //endregion

    //region Methods:
    private void updateNavButtonsStat() {
        View v = getView();
        if (v != null) {
            ImageButton btnNext = (ImageButton) v.findViewById(R.id.btn_next);
            ImageButton btnPrev = (ImageButton) v.findViewById(R.id.btn_prev);

            btnNext.setVisibility((isNextVisible() ? View.VISIBLE : View.INVISIBLE));
            btnPrev.setVisibility((isPrevVisible() ? View.VISIBLE : View.INVISIBLE));
        }
    }

    public void showMosqueSelectionStep() {
        MosqueSelectionFragment fragment = new MosqueSelectionFragment();
        fragment.setParentView(this);

        FragmentManager fragmentManager = getActivity().getSupportFragmentManager();
        FragmentTransaction transaction = fragmentManager.beginTransaction();
        transaction.replace(R.id.fragment_placeholder, fragment);
        transaction.commit();
    }

    public void showObitSelectionStep() {
        ObitSelectionFragment fragment = new ObitSelectionFragment();
        fragment.setParentView(this);

        FragmentManager fragmentManager = getActivity().getSupportFragmentManager();
        FragmentTransaction transaction = fragmentManager.beginTransaction();
        transaction.replace(R.id.fragment_placeholder, fragment);
        transaction.commit();
    }

    public void showTemplateSelectionStep() {
        TemplateSelectionFragment fragment = new TemplateSelectionFragment();
        fragment.setParentView(this);

        FragmentManager fragmentManager = getActivity().getSupportFragmentManager();
        FragmentTransaction transaction = fragmentManager.beginTransaction();
        transaction.replace(R.id.fragment_placeholder, fragment);
        transaction.commit();
    }

    public void showTemplateFieldsStep() {
        Toast.makeText(getActivity(), "show template fields fragment", Toast.LENGTH_SHORT).show();
    }
    //endregion

    //region Getters & Setters:
    public MosqueDto getSelectedMosque() {
        return selectedMosque;
    }

    public void setSelectedMosque(MosqueDto selectedMosque) {
        this.selectedMosque = selectedMosque;
    }

    public ObitDto getSelectedObit() {
        return selectedObit;
    }

    public void setSelectedObit(ObitDto selectedObit) {
        this.selectedObit = selectedObit;
    }

    public boolean isNextVisible() {
        return nextVisible;
    }

    public void setNextVisible(boolean nextVisible) {
        this.nextVisible = nextVisible;
        updateNavButtonsStat();
    }

    public boolean isPrevVisible() {
        return prevVisible;
    }

    public void setPrevVisible(boolean prevVisible) {
        this.prevVisible = prevVisible;
        updateNavButtonsStat();
    }

    public Runnable getOnNextClickListener() {
        return onNextClickListener;
    }

    public void setOnNextClickListener(Runnable onNextClickListener) {
        this.onNextClickListener = onNextClickListener;
    }

    public Runnable getOnPreviousClickListener() {
        return onPreviousClickListener;
    }

    public void setOnPreviousClickListener(Runnable onPreviousClickListener) {
        this.onPreviousClickListener = onPreviousClickListener;
    }

    public TemplateDto getSelectedTemplate() {
        return selectedTemplate;
    }

    public void setSelectedTemplate(TemplateDto selectedTemplate) {
        this.selectedTemplate = selectedTemplate;
    }

    //endregion

}